using System.Windows;
using System.Windows.Documents;

namespace Course_work_TIPIS
{
    /// <summary>
    ///Interection logic for AdvanceDicisionWindow.xaml
    /// </summary>
    public partial class AdvanceDicisionWindow : Window
    {
        public AdvanceDicisionWindow()
        {
            InitializeComponent();

            //Upload our solution here 
            SetSolutionSteps(FLDOC_AdvanceSolution, SECT_Tables, RUN_Answer);
        }

        #region Show all steps of solution
        
        //Set steps of solution in meet both paragraphs of FlowDocument
        private void SetSolutionSteps(FlowDocument _flowDocument, Section _tables, Run _resultedAnswer)
        {
            for(int i = 0; i < Service.solutionTables.Count; i++) {
                Paragraph p = new Paragraph();
                p.Inlines.Add(new Run("Итерация " + i + ":"));
                _tables.Blocks.Add(p);
                _tables.Blocks.Add(Service.solutionTables[i]);
            }

            _flowDocument.Blocks.Add(new Paragraph(new Run(Service.solutionString)));
            
        }
        #endregion
    }
}
