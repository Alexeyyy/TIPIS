using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Course_work_TIPIS
{
    //Class which provides a representation of input data, 
    //presents a conditions with adding of fective and artificial variables,
    //forms a data table for SimplexAlgorithm class.
    public static class DataHandling
    {
        private static int counterFectsNames = 0; //counter for names of fective variables (for correct naming)
        private static int counterArtsNames = 0; //counter for names of artificial variables 

        private static List<object[,]> addingArtificials = new List<object[,]>(); //list which concludes all conditions where artificial variables were added

        #region Read an input data

        //Get a signs of input conditions
        private static string[] GetConditionSigns(StackPanel _stackConditionsPanel, int _countConditions)
        {
            string[] conditionSigns = new string[_countConditions];

            for (int i = 0, currentElement = 0; i < _stackConditionsPanel.Children.Count; i++)
            {
                for (int j = 0; j < (_stackConditionsPanel.Children[i] as DockPanel).Children.Count; j++)
                {
                    if ((_stackConditionsPanel.Children[i] as DockPanel).Children[j] is ComboBox)
                    {
                        conditionSigns[currentElement] = ((_stackConditionsPanel.Children[i] as DockPanel).Children[j] as ComboBox).SelectionBoxItem.ToString();
                        currentElement++;                        
                    }
                }
            }
            
            return conditionSigns;
        }

        //Get a conditions coefficients 
        private static double[,] GetConditionsCoefficients(StackPanel _stackConditionPanel, int _countConditions, int _countVariables)
        {
            double[,] coefficientsMatrix = new double[_countConditions + 1, _countVariables + 1]; //+1 (1) - т.к. учитываем целевую ф-ю, +1 (2) - т.к. учитываем свободный член после знака равенства(неравенства)
                        
            for (int i = 0, currentI = 0; i < _stackConditionPanel.Children.Count; i++)
            {
                for (int j = 0, currentJ = 0; j < (_stackConditionPanel.Children[i] as DockPanel).Children.Count; j++)
                {
                    if ((_stackConditionPanel.Children[i] as DockPanel).Children[j] is TextBox)
                    {
                        coefficientsMatrix[currentI, currentJ] = Convert.ToDouble(((_stackConditionPanel.Children[i] as DockPanel).Children[j] as TextBox).Text);
                        currentJ++;
                    }
                }
                currentI++;
            }

            return coefficientsMatrix;
        }
        #endregion

        #region Presentation of conditions, target function in matrix view. An addition of artificial and fective variables.
        
        //Creation of target function matrix
        public static object[,] CreateTargetFunctionMatrix(StackPanel _stackConditionPanel, int _countConditions, int _countVariables)
        {
            double[,] conditionCoefficients = GetConditionsCoefficients(_stackConditionPanel, _countConditions, _countVariables);
            object[,] targetMatrix = new object[3, _countVariables];

            //fill the names of variables (x1, x2, ...) and coefficients too
            for (int j = 0; j < targetMatrix.GetLength(1); j++)
            {
                targetMatrix[0, j] = "x" + (j + 1);
                targetMatrix[1, j] = conditionCoefficients[0, j];
                targetMatrix[2, j] = 0; 
            }
                        
            return targetMatrix;
        }

        //An equations design (not inequalities, because we're adding a fective variables in the same time). The presentation in matrix view.
        public static void CreateEquationMatrix(StackPanel _stackConditionPanel, int _countConditions, int _countVariables, ref List<object[,]> _conditionSimplexMatrixes, ref List<object[,]> _conditionGraphicsMatrixes)
        {
            string[] conditionSigns = GetConditionSigns(_stackConditionPanel, _countConditions);
            double[,] conditionCoefficients = GetConditionsCoefficients(_stackConditionPanel, _countConditions, _countVariables); 
            bool f_DistributionSimplex = false;

            //The count of list elements is equal to count of conditions (k < _countConditions)
            for (int k = 0; k < _countConditions; k++)
            {
                object[,] matrixItem = null;
                int runningJ = 0;
                
                //If condition's sign is "=" then fective variables aren't added (_countVariables + 0)
                if (conditionSigns[k] == "=")
                {
                    matrixItem = new object[3, _countVariables + 1];
                    runningJ = _countVariables + 1;
                    f_DistributionSimplex = true;
                }
                
                //If condition's sign is "<=" or "=>" then fective variables are needed to add (_countVariables + 1)
                if (conditionSigns[k] == "<=" || conditionSigns[k] == "=>")
                {
                    matrixItem = new object[3, _countVariables + 2];
                    runningJ = _countVariables + 2;
                    f_DistributionSimplex = true;
                }

                //If condition's sign is "=" then fective variables aren't added (_countVariables + 0)
                if (conditionSigns[k] == ">" || conditionSigns[k] == "<")
                {
                    matrixItem = new object[3, _countVariables + 1];
                    runningJ = _countVariables + 1;
                    f_DistributionSimplex = false;
                }

                for (int j = 0; j < runningJ; j++)
                {
                    //i < 3, because there are three rows in matrix: the names of variables, left and right parts of equality
                    for (int i = 0; i < 3; i++)
                    {
                        //If it needs then fill fective variables
                        if (j >= _countVariables + 1)
                        {
                            //A name of the variable 
                            if (i == 0)
                            {
                                string fectName = "fect" + (j + counterFectsNames); //add a count of list's elements for supporting of numeral order of fective variables 
                                counterFectsNames++;
                                matrixItem[i, j] = fectName;
                            }
                            //A left part of equality 
                            if (i == 1)
                            {
                                //If the sign is "<=" then fective variable is equaled 1
                                if(conditionSigns[k] == "<=")
                                    matrixItem[i, j] = 1;
                                //If the sign is "<=" then fective variable is equaled 1
                                if (conditionSigns[k] == "=>")
                                    matrixItem[i, j] = -1;
                            }
                            //Consider that a right part of our equality equals 0
                            if (i == 2)
                                matrixItem[i, j] = 0;
                        }
                        //Work with a usual variables (x1,x2,x3 ...)
                        else
                        {
                            //A name of the variable
                            if (i == 0)
                                matrixItem[i,j] = "x" + (j + 1);
                            //A left part of equality 
                            if (i == 1)
                            {
                                //Because a free member in the left part of equality
                                if (j == _countVariables)
                                {
                                    matrixItem[0, j] = "Value"; //free member's name
                                    matrixItem[i, j] = conditionCoefficients[k + 1, j] * (-1);
                                }
                                //variables coefficients(x1,x2,...)
                                else
                                    matrixItem[i, j] = conditionCoefficients[k + 1, j];
                            }
                            //Consider the right part of equality is equaled 0
                            if (i == 2)
                                matrixItem[i, j] = 0;
                        }
                    }
                }

                //Add our condition in list for further handling by simplex method
                if(f_DistributionSimplex)
                    _conditionSimplexMatrixes.Add(matrixItem);
                //it's already unused part...
                /*else
                    _conditionGraphicsMatrixes.Add(matrixItem); */
            }
        }


        #region An addition of artificial variables

        //A checking of need and an addition of artificial variable
        private static bool CheckArtificialAddition(object[,] _conditionItem)
        {
            for (int j = 0; j < _conditionItem.GetLength(1); j++)
            {
                //if the fective variable is still found and equaled to -1
                if ((_conditionItem[0, j].ToString()).Contains("fect") && (int)_conditionItem[1, j] == -1)
                    return true;
                //if the fective variable is still found and equaled to 1
                if ((_conditionItem[0, j].ToString()).Contains("fect") && (int)_conditionItem[1, j] == 1)
                    return false;
            }

            //in case, if there are no any fective variables in the current condition
            return true;
        }

        //A checking on already added artifiacial variable
        private static bool CheckArtificialExistence(object[,] _conditionItem)
        {
            for (int j = 0; j < _conditionItem.GetLength(1); j++)
            {
                //If there is already artificial variable
                if ((_conditionItem[0, j].ToString()).Contains("art"))
                    return true;
            }

            //In an opposite way
            return false;
        }

        //An addition of artificial variables to elements of the list
        private static void AddArtificialVariableToItem(List<object[,]> _conditionSimplexMatrixes, object[,] _conditionItem, int _countVariables)
        {
            //In case of need to add a variable
            if (CheckArtificialAddition(_conditionItem) && !CheckArtificialExistence(_conditionItem))
            {
                //A foundation under new matrix with a place for artificial variable
                object[,] copyItem = new object[3, _conditionItem.GetLength(1) + 1];

                for (int j = 0; j < _conditionItem.GetLength(1); j++)
                {
                    for (int i = 0; i < _conditionItem.GetLength(0); i++)
                    {
                        copyItem[i, j] = _conditionItem[i, j];
                    }
                }

                //Add our artificial variable
                copyItem[0, copyItem.GetLength(1) - 1] = "art" + (counterArtsNames + counterFectsNames + 1 + _countVariables); //имя
                counterArtsNames++; //increment a counter for a name of the next artificial variable
                copyItem[1, copyItem.GetLength(1) - 1] = 1; //multiplier in the left part
                copyItem[2, copyItem.GetLength(1) - 1] = 0; //multiplier in the right part
                
                //Provide a replacement of list's element (this is about list for simplex-conditions)
                addingArtificials.Add(copyItem);
            }
            else
                addingArtificials.Add(_conditionItem);
        }
        
        //An addition of artificial variables to conditions in the list 
        public static void AddArtificialVariables(ref List<object[,]> _conditionSimplexMatrixes, int _countVariables)
        {
            for (int i = 0; i < _conditionSimplexMatrixes.Count; i++)
                AddArtificialVariableToItem(_conditionSimplexMatrixes, _conditionSimplexMatrixes[i], _countVariables); //формируем список addingArtificials

            _conditionSimplexMatrixes.Clear();
            Service.MakeSafeCopy(addingArtificials, _conditionSimplexMatrixes);
            
        }
        
        #endregion

        #endregion

        #region An expression of variables, artificial function and etc.

        //Find and return a list of conditions with artificial variables 
        private static List<object[,]> LookForConditionsWithArtificialVars(List<object[,]> _conditionSimplexMatrixes)
        {
            List<object[,]> conditionsWithArtVars = new List<object[,]>();

            //Provide a search
            for (int k = 0; k < _conditionSimplexMatrixes.Count; k++)
            {
                //Begin a running across a names of variables(from right side, because artificial variables are stored in the last cols of matrix)
                for (int j = _conditionSimplexMatrixes[k].GetLength(1) - 1; j >= 0 ; j--)
                {
                    //if the condition with artificial variable already exists then add it to list
                    if((_conditionSimplexMatrixes[k][0,j].ToString()).Contains("art")) 
                    {
                        conditionsWithArtVars.Add(_conditionSimplexMatrixes[k]);
                        break;
                    }
                }
            }
            
            return conditionsWithArtVars;
        }

        //Express an artificial variables 
        private static void ExpressArtifialVariables(ref List<object[,]> _conditionsWithArtVars)
        {
            for (int k = 0; k < _conditionsWithArtVars.Count; k++)
            {
                //Do a transfer of summands fromleft part to right
                for (int j = 0; j < _conditionsWithArtVars[k].GetLength(1); j++)
                {
                    //If the  variable isn't artificial then produce its transfer
                    if (!((_conditionsWithArtVars[k][0, j].ToString()).Contains("art")))
                    {
                        double tmpBox = Convert.ToDouble(_conditionsWithArtVars[k][1, j]);
                        _conditionsWithArtVars[k][1, j] = 0;
                        _conditionsWithArtVars[k][2, j] = Convert.ToDouble(_conditionsWithArtVars[k][2, j]) - tmpBox;
                    }
                }
            }
        }

        //Search for a names of fective variables which are taken part in creation of artificial function
        private static List<string> GetNamesFectiveVariables(List<object[,]> _conditionsWithArtVars)
        {
            List<string> actedFectiveVariables = new List<string>(); //an already used fective variables

            for (int k = 0; k < _conditionsWithArtVars.Count; k++)
            {
                //Seaarch for all fective variables
                for (int j = 0; j < _conditionsWithArtVars[k].GetLength(1); j++)
                {
                    //If the variable is fective than add it to list 
                    if ((_conditionsWithArtVars[k][0, j].ToString()).Contains("fect") && !actedFectiveVariables.Contains((_conditionsWithArtVars[k][0, j].ToString())))
                        actedFectiveVariables.Add(_conditionsWithArtVars[k][0, j].ToString());
                }
            }

            return actedFectiveVariables;
        }

        //Create and express an artificial fucntion
        private static object[,] ExpressArtificialFunction(List<object[,]> _conditionsWithArtVars, int _countVariables)
        {
            List<string> actedFectiveVariables = GetNamesFectiveVariables(_conditionsWithArtVars);
            object[,] artificialFunctionMatrix = new object[3, _countVariables + actedFectiveVariables.Count + 1];

            //(1) Form an artificial function on the foundation of shared summands
            //n < _countVariables + 1 (the count of variables + free member)
            for (int n = 0; n < _countVariables + 1; n++)
            {
                for (int k = 0; k < _conditionsWithArtVars.Count; k++)
                {
                    //Reduce the same summands in artificial function
                    if(k == 0)
                        artificialFunctionMatrix[0,n] = _conditionsWithArtVars[k][0,n];

                    artificialFunctionMatrix[1, n] = Convert.ToDouble(artificialFunctionMatrix[1, n]) + Convert.ToDouble(_conditionsWithArtVars[k][2, n]);
                    artificialFunctionMatrix[2, n] = 0; //Set a 0 to a right side
                }
            }
            
            //(2) Finish the forming process - add our fective variables 
            for (int j = _countVariables + 1, listCounter = 0; j < artificialFunctionMatrix.GetLength(1); j++, listCounter++)
            {
                artificialFunctionMatrix[0, j] = actedFectiveVariables[listCounter];
                artificialFunctionMatrix[1, j] = 1;
                artificialFunctionMatrix[2, j] = 0;
            }
            
            return artificialFunctionMatrix;
        }

        //Transfer our value in right part of artificial function (at the same time don't forget to consider a sign "-") 
        private static object[,] ChangeValueSide(object[,] _artificialFunction)
        {
            for (int j = 0; j < _artificialFunction.GetLength(1); j++)
            {
                if (_artificialFunction[0, j].ToString() == "Value")
                {
                    _artificialFunction[2,j] = (-1) * Convert.ToDouble(_artificialFunction[1, j]);
                    _artificialFunction[1, j] = 0;
                }
            }

            return _artificialFunction;
        }
        

        //An expression of artificial fucntion (by steps)
        public static object[,] GetArtificialFunctionMatrix(List<object[,]> _conditionSimplexMatrixes, int _countVariables)
        {
            List<object[,]> conditionsWithArtVars = LookForConditionsWithArtificialVars(_conditionSimplexMatrixes);
            //Express an artificial variables
            ExpressArtifialVariables(ref conditionsWithArtVars);

            return ChangeValueSide(ExpressArtificialFunction(conditionsWithArtVars, _countVariables));
        }
              
        #endregion

        #region Form a data table
        //Fill a title of the table and the first col which has a name "Base"
        private static void FillTableTitle(ref object[,] _simplexTable, int _countVariables)
        {
            //Fill the title of table data
            _simplexTable[0, 0] = "Base";
            _simplexTable[0, 1] = "Value";
            
            //Basic variables 
            for (int j = 2; j < _countVariables + 2; j++)
            {
                _simplexTable[0, j] = "x" + (j - 1);   
            }

            //Fective variables
            for (int j = _countVariables + 2; j < _countVariables + 2 + counterFectsNames; j++)
            {
                _simplexTable[0, j] = "fect" + (j - 1);   
            }

            //Artificial variables
            for (int j = _countVariables + 2 + counterFectsNames; j < _simplexTable.GetLength(1); j++)
            {
                _simplexTable[0, j] = "art" + (j - 1);   
            }
        }
        
        //Form a col "Base"
        private static void FillColumnBase(ref object[,] _simplexTable)
        {
            for (int i = 1; i < _simplexTable.GetLength(0) - 2; i++)
            {
                _simplexTable[i, 0] = "x";
            }

            _simplexTable[_simplexTable.GetLength(0) - 2, 0] = "-z";
            _simplexTable[_simplexTable.GetLength(0) - 1, 0] = "-w";
        }

        //Fill our simplex matrix by 0s
        private static void FillSimplexMatrixByNulls(ref object[,] _simplexTable)
        {
            for (int i = 1; i < _simplexTable.GetLength(0); i++)
                for (int j = 1; j < _simplexTable.GetLength(1); j++)
                    _simplexTable[i, j] = 0;
        }
                
        //Reduce all simplex-conditions to such view x1 + x2 + x3 + fect4 + art5 = Value
        private static void SetUsualView(ref List<object[,]> _conditionSimplexMatrixes)
        {
            for (int k = 0; k < _conditionSimplexMatrixes.Count; k++)
            {
                for (int j = 0; j < _conditionSimplexMatrixes[k].GetLength(1); j++)
                {
                    if (_conditionSimplexMatrixes[k][0, j].ToString() != "Value")
                    {
                        if (Convert.ToDouble(_conditionSimplexMatrixes[k][1, j]) == 0)
                        {
                            double tmpVar = Convert.ToDouble(_conditionSimplexMatrixes[k][2, j]) * (-1);
                            _conditionSimplexMatrixes[k][2, j] = 0;
                            _conditionSimplexMatrixes[k][1, j] = tmpVar;
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(_conditionSimplexMatrixes[k][1, j]) != 0)
                        {
                            double tmpVar = Convert.ToDouble(_conditionSimplexMatrixes[k][1, j]) * (-1);
                            _conditionSimplexMatrixes[k][2, j] = tmpVar;
                            _conditionSimplexMatrixes[k][1, j] = 0;
                        }
                    }
                }
            }
        }

        //Add a values through the searching of name 
        private static void SearchAndSetValue(object[,] _simplexTable, string _varName, double _value, int _rowIndex)
        {
            for (int j = 0; j < _simplexTable.GetLength(1); j++)
            {
                //If the name of varible was found
                if (_simplexTable[0, j].ToString() == _varName)
                {
                    _simplexTable[_rowIndex, j] = _value;
                    break;
                }
            }
        }

        //Fill the matrix by conditions coefficients and also target function's and artificial function's ones 
        private static void FillMatrixByCoefficients(ref object[,] _simplexTable, object[,] _targetFunctionMatrix, List<object[,]> _conditionSimplexMatrixes, object[,] _artificialFunctionMatrix)
        {
            _conditionSimplexMatrixes.Add(_targetFunctionMatrix);
            _conditionSimplexMatrixes.Add(_artificialFunctionMatrix);

            //A running across the rows of the table and list of conditions (the target and artificial fucntions are also included)
            for (int i = 1, listCounter = 0; i < _simplexTable.GetLength(0); i++, listCounter++)
            {
                for (int k = 0; k < _conditionSimplexMatrixes[listCounter].GetLength(1); k++)
                {
                    //If the coeffcient isn't a 0 then save it in the table (it means that current value isn't Value (table member))
                    if (Convert.ToDouble(_conditionSimplexMatrixes[listCounter][1, k]) != 0)
                        SearchAndSetValue(_simplexTable, _conditionSimplexMatrixes[listCounter][0, k].ToString(), Convert.ToDouble(_conditionSimplexMatrixes[listCounter][1, k]), listCounter + 1);
                    //If it's still equaled then check another side (in occasion of Value). If it's not a Value than will save 0 in both occasions
                    else
                        SearchAndSetValue(_simplexTable, _conditionSimplexMatrixes[listCounter][0, k].ToString(), Convert.ToDouble(_conditionSimplexMatrixes[listCounter][2, k]), listCounter + 1);
                }
            }

            //Clear the last two records of the list (the target and artificial functions records)
            _conditionSimplexMatrixes.RemoveRange(_conditionSimplexMatrixes.Count - 2, 2);
        }

        //The main method of forming a table data
        public static object[,] FormTableForSimplexMethod(object[,] _targetFunctionMatrix, List<object[,]> _conditionSimplexMatrixes, object[,] _artificialFunctionMatrix, int _countSimplexConditions, int _countVariables)
        {
            object[,] simplexTable = new object[_countSimplexConditions + 3, _countVariables + counterFectsNames + counterArtsNames + 2];
            FillTableTitle(ref simplexTable, _countVariables);
            FillColumnBase(ref simplexTable);
            FillSimplexMatrixByNulls(ref simplexTable);
            SetUsualView(ref _conditionSimplexMatrixes);
            FillMatrixByCoefficients(ref simplexTable, _targetFunctionMatrix, _conditionSimplexMatrixes, _artificialFunctionMatrix);

            ResetLocalVariables();
            
            return simplexTable;
        }

        //Produce a reset of local variables for the next calculations
        private static void ResetLocalVariables()
        {
            addingArtificials.Clear();
            counterFectsNames = 0;
            counterArtsNames = 0;
        }

        #endregion
    }
}
