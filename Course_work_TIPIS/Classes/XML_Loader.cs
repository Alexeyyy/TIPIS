using System;
using System.Xml;
using System.Diagnostics;

namespace Course_work_TIPIS
{
    public static class XML_Loader
    {
        public static object[,] dataArray;
        
        //Get our xml-document
        private static XmlDocument GetXmlDocument(string _xmlText)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //Check if last one exists than get a document. Otherwise, catch an exception
            try
            {
                xmlDoc.LoadXml(_xmlText);
                return xmlDoc;
            }
            catch
            {
                return null;
            }
            finally
            {
                //...
            }
        }

        //Search for needful record (If searching is succeful than get an objective XmlNode. Else return null)
        private static XmlNode SearchingObjectiveRecord(XmlNodeList _variants, int _variantNumber)
        {
            for(int i = 0; i < _variants.Count; i++) {
                
                if (Int32.Parse(_variants[i].Attributes["number"].Value) == _variantNumber)
                {
                    Trace.WriteLine(Int32.Parse(_variants[i].Attributes["number"].Value).ToString() + "!@#$%");
                    return _variants[i];
                }
                else
                    continue;
            }

            return null;
        }

        //Get a count of variables into variant
        private static int GetCountOfVarsInExample(XmlNodeList _expressions)
        {
            XmlNode node = _expressions[0]; //get the first node, because everyone has an attribute "coefficients" 
            string coefficientsString = node.Attributes["coefficients"].Value;

            //get a count of variables in variant
            return (coefficientsString.Split('|')).Length;
        }

        //Write all data about variant's expressions into "object[,]" array
        private static object[,] RecordData(XmlNodeList _expressions, object[,] _dataArray)
        {
            //consider a places for sign and free item (therefore "+2")
            _dataArray = new object[_expressions.Count, GetCountOfVarsInExample(_expressions) + 2];

            //It's really needful for further transformations and comfortability that the target function node would be the first in the dataArray.
            //The code below provides it
            for (int i = 0; i < _expressions.Count; i++)
            {
                if (_expressions[i].Name == "TargetFunction")
                {
                    string[] coefficients = (_expressions[i].Attributes["coefficients"].Value).Split('|');
                    //record coefficients in the dataArray
                    for (int j = 0, k = 0; j < _dataArray.GetLength(1); j++)
                        if (k < coefficients.Length)
                        {
                            _dataArray[0, j] = Convert.ToDouble(coefficients[k]);
                            k++;
                        }
                        //for avoiding of appearance of null-objects
                        else
                            _dataArray[0, j] = "nothing";
                }
            }

            //in case of rank condition
            for(int i = 0, k = 1; i < _expressions.Count; i++)
            {
                if (_expressions[i].Name != "TargetFunction")
                {
                    string[] coefficients = (_expressions[i].Attributes["coefficients"].Value).Split('|');
                    string sign = _expressions[i].Attributes["sign"].Value;
                    string freeMember = _expressions[i].Attributes["freeMember"].Value;
                    
                    //record coefficients, sign, member in the dataArray
                    int j = 0;
                    for (; j < coefficients.Length; j++)
                        _dataArray[k, j] = Convert.ToDouble(coefficients[j]);
                    
                    _dataArray[k, j ++] = sign;
                    _dataArray[k, j] = Convert.ToDouble(freeMember);
                    k++;
                }
            }

            return _dataArray;
        }
                
        //Filling dataArray by coefficients, arithmetical signs and etc.  
        private static bool FillDataArray(XmlDocument _xmlDocument, int _variantNumber, ref int _countConditions, ref int _countVariables)
        {
            XmlNodeList variants = _xmlDocument.GetElementsByTagName("Variant"); //get all records which marked by tag "Variant"
            XmlNode objectiveVariant = SearchingObjectiveRecord(variants, _variantNumber); //get a needful variant
            if (objectiveVariant != null)
            {
                XmlNodeList expressions = objectiveVariant.ChildNodes; //get a collection which includes all children nodes of objective variant
                dataArray = RecordData(expressions, dataArray);
                _countConditions = expressions.Count - 1;
                _countVariables = GetCountOfVarsInExample(expressions);

                return true;
            }
            else
                return false;
        }

        //public method for general using
        public static bool WorkWithXml(string _xmlText, int _variantNumber, ref int _countConditions, ref int _countVariables)
        {
            return FillDataArray(GetXmlDocument(_xmlText), _variantNumber, ref _countConditions, ref _countVariables);
        }
    }
}
