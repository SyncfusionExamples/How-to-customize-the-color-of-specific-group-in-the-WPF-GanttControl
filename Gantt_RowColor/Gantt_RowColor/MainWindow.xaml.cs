
namespace Gantt_RowColor
{
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Grid;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion
        
        #region Private methods

        private void Gantt_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Gantt.GanttGrid.Loaded += this.GanttGrid_Loaded;
        }

        private void GanttGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Gantt.GanttGrid.Model.QueryCellInfo += this.Model_QueryCellInfo;
            this.Gantt.GanttGrid.InternalGrid.PopulateTree();
            this.Gantt.GanttGrid.InternalGrid.ExpandAllNodes();
        }

        private void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            int nodeindex = this.Gantt.GanttGrid.InternalGrid.ResolveIndexToColumnIndex(e.Style.ColumnIndex);
            if (nodeindex > -1)
            {
                GridTreeNode node = this.Gantt.GanttGrid.InternalGrid.GetNodeAtRowIndex(e.Style.RowIndex);
                if (node != null && node.Item != null)
                {
                    Task task = node.Item as Task;
                    if (task != null)
                    {
                        if (task.RowType == RowType.Projectrow)
                        {
                            e.Style.Background = new SolidColorBrush(Colors.Red);
                        }
                        else if (task.RowType == RowType.SubProjectRow)
                        {
                            e.Style.Background = new SolidColorBrush(Colors.Yellow);
                        }
                        else if (task.RowType == RowType.AnalysisRow)
                        {
                            e.Style.Background = new SolidColorBrush(Colors.Orange);
                        }
                        else if (task.RowType == RowType.ProductionRow)
                        {
                            e.Style.Background = new SolidColorBrush(Colors.Green);
                        }
                        else if (task.RowType == RowType.TaskRow)
                        {
                            e.Style.Background = new SolidColorBrush(Colors.LightGray);
                        }
                    }
                }
            }
        }

        #endregion
    }
}