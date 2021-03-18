using System;
using System.ComponentModel;
using System.Linq;

namespace Servize.Utility.QueryFilter
{
    class QueryFilterParser
    {
        public string Filter { get; set; }
        public string ProcessedString { get; set; }

        [DefaultValue(false)]
        public bool CloseBracketFound { get; set; }

        [DefaultValue(false)]
        public bool OpenBracketFound { get; set; }

        [DefaultValue(false)]
        public bool FirstEqualFound { get; set; }

        [DefaultValue(false)]
        public bool CharIFound { get; set; }

        [DefaultValue(false)]
        public bool CharNFound { get; set; }

        [DefaultValue(false)]
        public bool CharOFound { get; set; }

        [DefaultValue(false)]
        public bool CharUFound { get; set; }

        [DefaultValue(false)]
        public bool CharTFound { get; set; }

        [DefaultValue(false)]
        public bool SecondEqualFound { get; set; }

        [DefaultValue(-1)]
        public int CloseBracketIndex { get; set; }

        [DefaultValue("")]
        public string BracketValueString { get; set; }

        [DefaultValue("")]
        public string PropString { get; set; }

        internal string ParseData(string filter)
        {
            for (int i = filter.Length - 1; i >= 0; i--)
            {
                char c = filter.ElementAt(i);
                if (CheckCloseBracket(c, i))
                {

                }
                else if (CheckOpenBracket(c))
                {
                    continue;
                }
                else if (CloseBracketFound && !OpenBracketFound)
                {
                    BracketValueString = BracketValueString.Insert(0, c.ToString());
                }

                if (ParseCharacter(c, i)) continue;
            }

            return ProcessedString;
        }

        private bool ParseCharacter(char c, int i)
        {
            if (OpenBracketFound)
            {
                if (Char.IsWhiteSpace(c)) return true;
                if (CheckFirstEqual(c)) return true;
                if (CheckCharN(c)) return true;
                if (CheckCharI(c)) return true;
                if (CheckCharT(c)) return true;
                if (CheckCharU(c)) return true;
                if (CheckCharO(c)) return true;
                CheckSecondEqual(c);
                ParseParameters(ref i);
                ResetProperties();
                return true;
            }

            return false;
        }

        private bool CheckCloseBracket(char c, int i)
        {
            if (c == ')')
            {
                OpenBracketFound = false;
                CloseBracketFound = true;
                CloseBracketIndex = i;
                BracketValueString = "";
                return true;
            }
            return false;
        }

        private bool CheckOpenBracket(char c)
        {
            if (c == '(' && CloseBracketFound && !OpenBracketFound)
            {
                OpenBracketFound = true;
                return true;
            }

            return false;
        }

        private bool CheckFirstEqual(char c)
        {
            if (c == '=' && !FirstEqualFound)
            {
                FirstEqualFound = true;
                return true;
            }
            return false;
        }

        private bool CheckCharN(char c)
        {
            if (FirstEqualFound && c == 'n' && !CharNFound)
            {
                CharNFound = true;
                return true;
            }
            return false;
        }

        private bool CheckCharI(char c)
        {
            if (CharNFound && c == 'i' && !CharIFound)
            {
                CharIFound = true;
                return true;
            }
            return false;
        }

        private bool CheckCharT(char c)
        {
            if (FirstEqualFound && c == 't' && !CharTFound)
            {
                CharTFound = true;
                return true;
            }
            return false;
        }

        private bool CheckCharU(char c)
        {
            if (CharTFound && c == 'u' && !CharUFound)
            {
                CharUFound = true;
                return true;
            }
            return false;
        }

        private bool CheckCharO(char c)
        {
            if (CharTFound && CharUFound && c == 'o' && !CharOFound)
            {
                CharOFound = true;
                return true;
            }
            return false;
        }

        private bool CheckSecondEqual(char c)
        {
            if ((CharIFound || CharOFound) && c == '=' && !SecondEqualFound)
            {
                SecondEqualFound = true;
                return true;
            }
            return false;
        }

        private bool ParseParameters(ref int i)
        {
            bool inQuery = (CharNFound && CharIFound && !CharOFound && !CharUFound && !CharTFound);
            bool outQuery = (!CharNFound && !CharIFound && CharOFound && CharUFound && CharTFound);
            if (FirstEqualFound && (inQuery || outQuery) && SecondEqualFound)
            {
                int lastProcessedIndex = -1;
                PropString = GetPropertyName(i - 1, ref lastProcessedIndex);
                if (lastProcessedIndex != -1)
                    i = lastProcessedIndex;
                ReplaceInString(PropString, lastProcessedIndex, inQuery);
                return true;
            }

            return false;
        }

        private string GetPropertyName(int index, ref int lastProcessedIndex)
        {
            string propertyName = "";
            bool propNameStarted = false;
            for (int i = index; i >= 0; i--)
            {
                char currentChar = Filter.ElementAt(i);
                if (Char.IsWhiteSpace(currentChar) && !propNameStarted)
                {
                    continue;
                }
                else
                {
                    propNameStarted = true;
                }

                if (!Char.IsWhiteSpace(currentChar) && currentChar != ',' && currentChar != ';' && currentChar != ')' && currentChar != '(')
                {
                    propertyName = propertyName.Insert(0, currentChar.ToString());
                    lastProcessedIndex = i;
                }
                else
                {
                    lastProcessedIndex = i + 1;
                    return propertyName;
                }
            }

            return propertyName;
        }

        private void ReplaceInString(string propName, int replaceFrom, bool inQuery)
        {
            if (String.IsNullOrEmpty(propName) || String.IsNullOrEmpty(BracketValueString) || replaceFrom >= CloseBracketIndex || replaceFrom < 0 || CloseBracketIndex > ProcessedString.Length)
                return;

            string[] valueArray = BracketValueString.Split(",");
            if (!valueArray.Any())
                return;

            string inString = ProcessInString(ref propName, inQuery, valueArray);
            ProcessedString = ProcessedString.Remove(replaceFrom, ((CloseBracketIndex + 1) - replaceFrom));
            ProcessedString = ProcessedString.Insert(replaceFrom, inString);
        }

        private string ProcessInString(ref string propName, bool inQuery, string[] valueArray)
        {
            string retVal = "";
            propName = propName.Trim();
            foreach (string value in valueArray)
            {
                string trimValue = value.Trim();
                if (retVal != "")
                {
                    if (inQuery)
                        retVal += (" or ");
                    else
                        retVal += (" and ");
                }

                string temp = "";
                if (inQuery)
                    temp = propName + " == " + trimValue;
                else
                    temp = propName + " != " + trimValue;

                retVal += temp;
            }

            retVal = retVal.Insert(0, "(");
            retVal = retVal.Insert(retVal.Length, ")");
            return retVal;
        }

        private void ResetProperties()
        {
            CloseBracketFound = false;
            OpenBracketFound = false;
            FirstEqualFound = false;
            CharNFound = false;
            CharIFound = false;
            CharOFound = false;
            CharUFound = false;
            CharTFound = false;
            SecondEqualFound = false;
            CloseBracketIndex = -1;
            BracketValueString = "";
        }
    }
}
