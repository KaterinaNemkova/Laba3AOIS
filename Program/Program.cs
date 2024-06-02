using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;



namespace laba2AOIS
{
    public class LogicFunctions
    {
        public static bool OperationAnd(bool a, bool b)
        {
            return a && b;
        }

        public static bool OperationOr(bool a, bool b)
        {
            return a || b;
        }

        public static bool OperationNot(bool a)
        {
            return !a;
        }

        public static bool OperationImplication(bool a, bool b)
        {
            return a ? b : true;
        }

        public static bool OperationEquivalence(bool a, bool b)
        {
            return (a && b) || (!a && !b);
        }

        private static int Prioritize(char op)
        {
            if (op == '!')
                return 3;
            if (op == '&' || op == '|')
                return 2;
            if (op == '>' || op == '~')
                return 1;
            return 0;
        }

        public static string OPZ(string s)
        {
            Stack<char> st = new Stack<char>();
            string opz = "";
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (Char.IsLetter(c))
                {
                    opz += c;
                }

                else if (c == '&' || c == '|' || c == '!' || c == '>' || c == '~')
                {
                    while (st.Count > 0 && Prioritize(s[i]) <= Prioritize(st.Peek()))
                    {
                        opz += st.Pop();
                    }
                    st.Push(c);
                }

                else if (c == '(')
                {
                    st.Push(c);
                }

                else if (c == ')')
                {
                    while (st.Count > 0 && st.Peek() != '(')
                    {
                        opz += st.Pop();
                    }
                    st.Pop();
                }
            }

            while (st.Count > 0)
            {
                opz += st.Pop();
            }

            return opz;
        }

        public static void PrintLogicTable(int n, string expression)
        {
            string postfixExpression = OPZ(expression);
            List<List<bool>> truthTable = GenerateTruthTable(n, expression, postfixExpression);
            string SKNF = "";
            string SDNF = "";
            List<int> sknfIndices = new List<int>();
            List<int> sdnfIndices = new List<int>();
            List<int> decimalResult = new List<int>();

            PopulateResultsAndIndices(truthTable, postfixExpression, n, ref SKNF, ref SDNF, sknfIndices, sdnfIndices, decimalResult);

            PrintResults(expression, postfixExpression, truthTable, SKNF, SDNF, sknfIndices, sdnfIndices, decimalResult);



            string minimizeSKNF = MinimizeSKNF_Table(SKNF);
            string minimizeSDNF = MinimizeSDNF_Table(SDNF);

            MinimizeSKNFbyKarnaugh(SKNF, decimalResult);
            Console.WriteLine(minimizeSKNF);

            MinimizeSDNFbyKarnaugh(SDNF, decimalResult);
            Console.WriteLine(minimizeSDNF);
        }

        public static List<List<bool>> GenerateTruthTable(int n, string expression, string postfixExpression)
        {
            int Rows = (int)Math.Pow(2, n);
            List<List<bool>> truthTable = new List<List<bool>>();

            for (int i = 0; i < Rows; ++i)
            {
                List<bool> values = new List<bool>();
                for (int j = 0; j < n; ++j)
                {
                    values.Insert(0, (i & (1 << j)) != 0);
                }
                truthTable.Add(values);
            }

            return truthTable;
        }

        public static void PopulateResultsAndIndices(List<List<bool>> truthTable, string postfixExpression, int n, ref string SKNF, ref string SDNF, List<int> sknfIndices, List<int> sdnfIndices, List<int> decimalResult)
        {
            for (int rowIndex = 0; rowIndex < truthTable.Count; rowIndex++)
            {
                List<bool> row = truthTable[rowIndex];
                List<bool> results = CalculatePostfixExpression(postfixExpression, row);
                UpdateSKNFAndSDNF(results, row, n, rowIndex, ref SKNF, ref SDNF, sknfIndices, sdnfIndices);

                decimalResult.Add(results[results.Count - 1] ? 1 : 0);
            }
        }

        public static void PrintResults(string expression, string postfixExpression, List<List<bool>> truthTable, string SKNF, string SDNF, List<int> sknfIndices, List<int> sdnfIndices, List<int> decimalResult)
        {
            Console.WriteLine("Truth Table:");
            foreach (char c in expression)
            {
                if (Char.IsLetter(c))
                {
                    Console.Write(c + "\t");
                }
            }
            foreach (char c in postfixExpression)
            {
                if (c == '&' || c == '|' || c == '!' || c == '>' || c == '~')
                {
                    Console.Write(c + "\t");
                }
            }
            Console.WriteLine();

            foreach (var row in truthTable)
            {
                foreach (bool value in row)
                {
                    Console.Write((value ? "1" : "0") + "\t");
                }

                List<bool> results = CalculatePostfixExpression(postfixExpression, row);
                foreach (bool result in results)
                {
                    Console.Write((result ? "1" : "0") + "\t");
                }
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(SKNF)) SKNF = SKNF.Substring(0, SKNF.Length - 3);
            if (!string.IsNullOrEmpty(SDNF)) SDNF = SDNF.Substring(0, SDNF.Length - 3);

            Console.WriteLine("SKNF: " + SKNF);
            Console.Write("SKNF Indices: ");
            foreach (int index in sknfIndices)
            {
                Console.Write(index + " ");
            }
            Console.WriteLine();

            Console.WriteLine("SDNF: " + SDNF);
            Console.Write("SDNF Indices: ");
            foreach (int index in sdnfIndices)
            {
                Console.Write(index + " ");
            }
            Console.WriteLine();

            int binaryResult = ConvertBinaryToDecimal(decimalResult);
            Console.WriteLine("Decimal result: " + binaryResult);

           

        }
        private static void UpdateSKNFAndSDNF(List<bool> results, List<bool> values, int n, int rowIndex, ref string SKNF, ref string SDNF, List<int> sknfIndices, List<int> sdnfIndices)
        {
            if (results[results.Count - 1] == false)
            {
                SKNF += ConstructSKNF(values, n);
                sknfIndices.Add(rowIndex);
            }
            else
            {
                SDNF += ConstructSDNF(values, n);
                sdnfIndices.Add(rowIndex);
            }
        }

        public static string ConstructSKNF(List<bool> values, int n)
        {
            string sknf = "(";
            for (int j = 0; j < n; ++j)
            {
                sknf += (values[j] ? "!" : "") + (char)('a' + j) + (j < n - 1 ? " | " : "");
            }
            sknf += ") & ";
            return sknf;
        }

        public static string ConstructSDNF(List<bool> values, int n)
        {
            string sdnf = "(";
            for (int j = 0; j < n; ++j)
            {
                sdnf += (values[j] ? "" : "!") + (char)('a' + j) + (j < n - 1 ? " & " : "");
            }
            sdnf += ") | ";
            return sdnf;
        }
        public static List<bool> CalculatePostfixExpression(string postfixExpression, List<bool> values)
        {
            Stack<bool> st = new Stack<bool>();
            List<bool> results = new List<bool>();
            foreach (char c in postfixExpression)
            {
                if (Char.IsLetter(c))
                {
                    st.Push(values[c - 'a']);
                }
                else
                {
                    bool result = false;
                    bool operand2 = st.Pop();
                    if (c == '!')
                    {
                        result = OperationNot(operand2);
                    }
                    else
                    {
                        bool operand1 = st.Pop();
                        if (c == '&')
                        {
                            result = OperationAnd(operand1, operand2);
                        }
                        else if (c == '|')
                        {
                            result = OperationOr(operand1, operand2);
                        }
                        else if (c == '~')
                        {
                            result = OperationEquivalence(operand1, operand2);
                        }
                        else if (c == '>')
                        {
                            result = OperationImplication(operand1, operand2);
                        }
                    }
                    st.Push(result);
                    results.Add(result);
                }
            }
            return results;
        }

        public static int ConvertBinaryToDecimal(List<int> binaryResult)
        {
            int decimalValue = 0;
            int baseValue = 1;


            for (int i = binaryResult.Count - 1; i >= 0; --i)
            {
                if (binaryResult[i] == 1)
                {
                    decimalValue += baseValue;
                }
                baseValue *= 2;
            }

            return decimalValue;
        }

        public static List<string> GetClauses(string sknf)
        {
            List<string> clauses = new List<string>();
            int openBracketIndex = 0;
            int balance = 0;

            for (int i = 0; i < sknf.Length; i++)
            {
                if (sknf[i] == '(')
                {
                    if (balance == 0)
                    {
                        openBracketIndex = i;
                    }
                    balance++;
                }
                else if (sknf[i] == ')')
                {
                    balance--;
                    if (balance == 0)
                    {
                        string clause = sknf.Substring(openBracketIndex, i - openBracketIndex + 1);
                        clauses.Add(clause);
                    }
                }
            }

            return clauses;
        }

        
        private static bool AreLettersEqual(List<string> literals1, List<string> literals2)
        {
            // Преобразуем список литералов в список букв, игнорируя знаки "!"
            var letters1 = literals1.Select(literal => literal.TrimStart('!')).ToList();
            var letters2 = literals2.Select(literal => literal.TrimStart('!')).ToList();

            // Сравниваем буквы в списках
            return letters1.OrderBy(letter => letter).SequenceEqual(letters2.OrderBy(letter => letter));
        }

        static List<string> GetLiterals(string clause)
        {
            List<string> literals = new List<string>();
            for (int i = 0; i < clause.Length; i++)
            {
                char currentChar = clause[i];
                if (char.IsLetter(currentChar))
                {
                    literals.Add(currentChar.ToString());
                }


                // Если текущий символ - отрицательный знак '!'
                else if (currentChar == '!')
                {
                    // Создаем строку, объединяя отрицательный знак и следующую за ним букву
                    string literal = currentChar.ToString() + clause[i + 1];

                    // Добавляем объединенную строку в список литералов
                    literals.Add(literal);

                    // Пропускаем следующий символ, так как мы уже обработали его
                    i++;
                }

            }

            return literals;
        }

        public static bool IsSubstringInOriginal(string subConstituent, string originalConstituent)
        {
            List<string> subLiterals = SplitImplicants(subConstituent);
            List<string> originalLiterals = SplitImplicants(originalConstituent);

            foreach (string literal in subLiterals)
            {
                if (!originalLiterals.Contains(literal))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<string> SplitImplicants(string input)
        {
            List<string> result = new List<string>();
            string temp = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '!')
                {
                    temp += input[i];
                    continue;
                }
                if (input[i] == '|' || input[i] == '(' || input[i] == ')' || input[i] == ' ' || input[i] == '&')
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        result.Add(temp);
                        temp = "";
                    }
                    continue;
                }
                temp += input[i];
            }
            if (!string.IsNullOrEmpty(temp))
            {
                result.Add(temp);
            }
            return result;
        }
        public static string MinimizeSKNF_Table(string sknf)
        {
            List<string> clauses = GetClauses(sknf);
            if (clauses == null || clauses.Count <= 1)
            {
                return clauses != null && clauses.Count == 1 ? clauses[0] : "";
            }

            StringBuilder result = new StringBuilder();
            bool changed = true;
            int stage = 1;
            while (changed)
            {
                bool merged = false;
                List<string> newClauses = new List<string>();
                changed = false;

                Console.WriteLine("Stage " + stage + ":");

                for (int i = 0; i < clauses.Count; ++i)
                {
                    merged = false;
                    for (int j = 0; j < clauses.Count; ++j)
                    {
                        if (i != j)
                        {
                            List<string> literals1 = GetLiterals(clauses[i]);
                            List<string> literals2 = GetLiterals(clauses[j]);

                            List<string> matchingLiterals = new List<string>();
                            if (AreLettersEqual(literals1, literals2))
                            {
                                for (int k = 0; k < literals1.Count; ++k)
                                {
                                    if (literals2.Contains(literals1[k]))
                                    {
                                        matchingLiterals.Add(literals1[k]);
                                    }
                                }

                                if (matchingLiterals.Count >= literals1.Count - 1)
                                {
                                    string mergedClause = string.Join(" & ", matchingLiterals);
                                    if (!newClauses.Contains(mergedClause))
                                    {
                                        newClauses.Add(mergedClause);
                                        changed = true;
                                        Console.WriteLine("Merged: " + clauses[i] + " with " + clauses[j] + " -> " + mergedClause);
                                    }
                                    merged = true;
                                }
                            }
                        }
                    }
                    if (!merged)
                    {
                        newClauses.Add(clauses[i]);
                    }
                }
                clauses = newClauses;
                stage++;
            }

            for (int i = 0; i < clauses.Count; ++i)
            {
                result.Append("(" + clauses[i] + ")");
                if (i < clauses.Count - 1)
                {
                    result.Append(" & ");
                }
            }


            Console.WriteLine("Table of Merged Clauses:");
            List<string> originalClauses = GetClauses(sknf);
            Console.Write(string.Format("{0,15}", ""));
            foreach (var clause in originalClauses)
            {
                Console.Write(string.Format("{0,15}", clause));
            }
            Console.WriteLine();

            for (int i = 0; i < clauses.Count; ++i)
            {
                Console.Write(string.Format("{0,15}", clauses[i]));
                for (int j = 0; j < originalClauses.Count; ++j)
                {

                    bool containsSubstring = IsSubstringInOriginal(clauses[i], originalClauses[j]);
                    if (containsSubstring)
                    {
                        Console.Write(string.Format("{0,15}", "X"));
                    }
                    else
                    {
                        Console.Write(string.Format("{0,15}", ""));
                    }


                }
                Console.WriteLine();
            }

            return result.ToString();
        }


        public static string MinimizeSDNF_Table(string sdnf)
        {
            List<string> clauses = GetClauses(sdnf);
            if (clauses == null || clauses.Count <= 1)
            {
                return clauses != null && clauses.Count == 1 ? clauses[0] : "";
            }

            StringBuilder result = new StringBuilder();
            bool changed = true;
            int stage = 1;
            while (changed)
            {
                bool merged = false;
                List<string> newClauses = new List<string>();
                changed = false;

                Console.WriteLine("Stage " + stage + ":");

                for (int i = 0; i < clauses.Count; ++i)
                {
                    merged = false;
                    for (int j = 0; j < clauses.Count; ++j)
                    {
                        if (i != j)
                        {
                            List<string> literals1 = GetLiterals(clauses[i]);
                            List<string> literals2 = GetLiterals(clauses[j]);

                            List<string> matchingLiterals = new List<string>();
                            if (AreLettersEqual(literals1, literals2))
                            {
                                for (int k = 0; k < literals1.Count; ++k)
                                {
                                    if (literals2.Contains(literals1[k]))
                                    {
                                        matchingLiterals.Add(literals1[k]);
                                    }
                                }

                                if (matchingLiterals.Count >= literals1.Count - 1)
                                {
                                    string mergedClause = string.Join(" & ", matchingLiterals);
                                    if (!newClauses.Contains(mergedClause))
                                    {
                                        newClauses.Add(mergedClause);
                                        changed = true;
                                        Console.WriteLine("Merged: " + clauses[i] + " with " + clauses[j] + " -> " + mergedClause);
                                    }
                                    merged = true;
                                }
                            }
                        }
                    }
                    if (!merged)
                    {
                        newClauses.Add(clauses[i]);
                    }
                }
                clauses = newClauses;
                ++stage;
            }

            for (int i = 0; i < clauses.Count; ++i)
            {
                result.Append("(" + clauses[i] + ")");
                if (i < clauses.Count - 1)
                {
                    result.Append(" | ");
                }
            }

            Console.WriteLine("Table of Merged Clauses:");
            List<string> originalClauses = GetClauses(sdnf);
            Console.Write(string.Format("{0,15}", ""));
            foreach (var clause in originalClauses)
            {
                Console.Write(string.Format("{0,15}", clause));
            }
            Console.WriteLine();

            for (int i = 0; i < clauses.Count; ++i)
            {
                Console.Write(string.Format("{0,15}", clauses[i]));
                for (int j = 0; j < originalClauses.Count; ++j)
                {
                    bool containsSubstring = IsSubstringInOriginal(clauses[i], originalClauses[j]);
                    if (containsSubstring)
                    {
                        Console.Write(string.Format("{0,15}", "X"));
                    }
                    else
                    {
                        Console.Write(string.Format("{0,15}", ""));
                    }


                }
                Console.WriteLine();
            }

            return result.ToString();
        }

        public static int ToGray(int number)
        {
            return number ^ (number >> 1);
        }

        public static List<string> ExtractVariablesSKNF(string sknf)
        {
            var variables = new HashSet<string>();
            var term = new StringBuilder();
            bool insideTerm = false;

            foreach (char ch in sknf)
            {
                if (ch == '(')
                {
                    insideTerm = true;
                    term.Clear();
                }
                else if (ch == ')')
                {
                    insideTerm = false;
                    if (term.Length > 0)
                    {
                        var vars = term.ToString().Split('|').Select(v => v.Trim().Trim('!')).Where(v => !string.IsNullOrEmpty(v));
                        foreach (var var in vars)
                        {
                            variables.Add(var);
                        }
                    }
                }
                else if (insideTerm)
                {
                    term.Append(ch);
                }
            }

            return variables.ToList();
        }

        public static List<string> ExtractVariablesSDNF(string sdnf)
        {
            var variables = new HashSet<string>();
            var term = new StringBuilder();
            bool insideTerm = false;

            foreach (char ch in sdnf)
            {
                if (ch == '(')
                {
                    insideTerm = true;
                    term.Clear();
                }
                else if (ch == ')')
                {
                    insideTerm = false;
                    if (term.Length > 0)
                    {
                        var vars = term.ToString().Split('&').Select(v => v.Trim().Trim('!')).Where(v => !string.IsNullOrEmpty(v));
                        foreach (var var in vars)
                        {
                            variables.Add(var);
                        }
                    }
                }
                else if (insideTerm)
                {
                    term.Append(ch);
                }
            }

            return variables.ToList();
        }

        public static void MinimizeSKNFbyKarnaugh(string sknf, List<int> index)
        {
            var variables = ExtractVariablesSKNF(sknf);
            int n = variables.Count;
            int rows, cols;

            if (n % 2 == 0)
            {
                rows = cols = 1 << (n / 2);
            }
            else
            {
                rows = 1 << (n / 2);
                cols = 1 << ((n + 1) / 2);
            }

            var karnaughMap = new int[rows, cols];
            for (int i = 0; i < index.Count; i++)
            {
                int row = ToGray(i / cols);
                int col = ToGray(i % cols);
                karnaughMap[row, col] = index[i];
            }

            Console.WriteLine("Karnaugh Map:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(karnaughMap[i, j] + "   ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public static void MinimizeSDNFbyKarnaugh(string sdnf, List<int> index)
        {
            var variables = ExtractVariablesSDNF(sdnf);
            int n = variables.Count;
            int rows, cols;

            if (n % 2 == 0)
            {
                rows = cols = 1 << (n / 2);
            }
            else
            {
                rows = 1 << (n / 2);
                cols = 1 << ((n + 1) / 2);
            }

            var karnaughMap = new int[rows, cols];
            for (int i = 0; i < index.Count; i++)
            {
                int row = ToGray(i / cols);
                int col = ToGray(i % cols);
                karnaughMap[row, col] = index[i];
            }

            var sb = new StringBuilder();
            sb.AppendLine("Karnaugh Map for SDNF:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(karnaughMap[i, j] + "   ");
                }
                sb.AppendLine().AppendLine();
            }
            Console.Write(sb.ToString());
        }



    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter expression : ");
            string expression = Console.ReadLine();

            HashSet<char> variables = new HashSet<char>();
            foreach (char c in expression)
            {
                if (Char.IsLetter(c))
                {
                    variables.Add(c);
                }
            }

            LogicFunctions.PrintLogicTable(variables.Count, expression);

            Console.ReadLine();
        }
    }


}