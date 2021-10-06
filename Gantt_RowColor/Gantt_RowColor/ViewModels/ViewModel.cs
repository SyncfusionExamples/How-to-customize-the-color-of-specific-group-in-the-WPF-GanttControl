
namespace Gantt_RowColor
{
    using System;
    using System.Collections.ObjectModel;

    public class ViewModel
    {
        #region Fields
        
        private int id = 0;
        private DateTime date;
        private ObservableCollection<Task> _taskDetails;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.date = DateTime.Today;
            this._taskDetails = this.GetData();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the appointment item source.
        /// </summary>
        /// <value>The appointment item source.</value>
        public ObservableCollection<Task> TaskDetails
        {
            get
            {
                return this._taskDetails;
            }
            set
            {
                this._taskDetails = value;
            }
        }

        #endregion

        #region Private methods
        
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Task> GetData()
        {
            var data = new ObservableCollection<Task>();

            data.Add(new Task { Id = this.id++, RowType = RowType.Projectrow, Name = "Project Row", StDate = this.date, EndDate = this.date.AddDays(2) });
            data[0].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.SubProjectRow, Name = "Sub Project Row", StDate = this.date, EndDate = this.date.AddDays(2) });
            data[0].ChildTask[0].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.AnalysisRow, Name = "Analysis Row", StDate = this.date, EndDate = this.date.AddDays(2) });
            data[0].ChildTask[0].ChildTask[0].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.ProductionRow, Name = "Production Row", StDate = this.date, EndDate = this.date.AddDays(2) });

            data[0].ChildTask[0].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.AnalysisRow, Name = "Analysis Row", StDate = this.date, EndDate = this.date.AddDays(2) });
            data[0].ChildTask[0].ChildTask[1].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.ProductionRow, Name = "Production Row", StDate = this.date, EndDate = this.date.AddDays(2) });
            data[0].ChildTask[0].ChildTask[1].ChildTask[0].ChildTask.Add(new Task { Id = this.id++, RowType = RowType.TaskRow, Name = "Task Row", StDate = this.date, EndDate = this.date.AddDays(2) });

            return data;
        }

        #endregion
    }
}