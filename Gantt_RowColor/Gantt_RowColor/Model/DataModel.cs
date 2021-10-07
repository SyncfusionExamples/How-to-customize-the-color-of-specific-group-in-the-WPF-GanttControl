
namespace Gantt_RowColor
{
    using System;
    using System.Linq;
    using Syncfusion.Windows.Shared;
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.Controls.Gantt;
    using System.ComponentModel;
    using System.Collections.Specialized;

    public class Task : NotificationObject
    {
        #region Fields

        private int id;
        private string name;
        private DateTime stDate;
        private DateTime endDate;
        private TimeSpan duration;
        private ObservableCollection<Resource> resource;
        private double complete;
        private ObservableCollection<Task> childTask;
        private ObservableCollection<Predecessor> predecessor;
        private RowType rowType;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        public Task()
        {
            ChildTask = new ObservableCollection<Task>();
            Predecessor = new ObservableCollection<Predecessor>();
            Resource = new ObservableCollection<Resource>();
        }

        #endregion

        #region Public properties

        public RowType RowType
        {
            get
            {
                return this.rowType;
            }

            set
            {
                this.rowType = value;
                this.RaisePropertyChanged("RowType");
            }
        }

        /// <summary>
        /// Gets or sets the complete.
        /// </summary>
        /// <value>
        /// The complete.
        /// </value>
        public double Complete
        {
            get
            {
                return Math.Round(this.complete, 2);
            }

            set
            {
                if (value <= 100)
                {
                    if (this.childTask != null && this.childTask.Count >= 1)
                    {
                        var sum = 0d;
                        this.complete = this.childTask.Aggregate(sum, (cur, task) => cur + task.Complete) / this.childTask.Count;
                    }
                    else
                    {
                        this.complete = value;
                    }

                    this.RaisePropertyChanged("Complete");
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>
        /// The resource.
        /// </value>
        public ObservableCollection<Resource> Resource
        {
            get
            {
                return this.resource;
            }

            set
            {
                this.resource = value;
                this.RaisePropertyChanged("Resource");
            }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration
        {
            get
            {
                if (this.childTask != null && this.childTask.Count >= 1)
                {
                    var sum = new TimeSpan(0, 0, 0, 0);
                    sum = this.childTask.Aggregate(sum, (current, task) => current + task.Duration);
                    return sum;
                }

                /// The Difference Between the EndDate and StartDate is Calculated exactly.
                this.duration = this.endDate.Subtract(stDate);
                return this.duration;
            }

            set
            {
                if (this.childTask != null && this.childTask.Count >= 1)
                {
                    var sum = new TimeSpan(0, 0, 0, 0);
                    sum = this.childTask.Aggregate(sum, (current, task) => current + task.Duration);
                    this.duration = sum;
                    return;
                }

                this.duration = value;

                // End date is being calcuated here to make the change in endate based on duration.
                // Duration is interlinked with start and end date, so will affect both based on the change.
                this.EndDate = this.stDate.AddDays(Double.Parse(this.duration.TotalDays.ToString()));
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                if (this.childTask != null && this.childTask.Count >= 1)
                {
                    /// If this task is a parent task, then it should have the maximum end time. Hence comparing the date with maximum date of its children.

                    if (value >= this.childTask.Max(s => s.EndDate) && this.endDate != value)
                    {
                        this.endDate = value;
                    }
                }
                else
                {
                    endDate = value;
                }

                this.RaisePropertyChanged("EndDate");

                // Duration changed is invoked to notify the chagne in duration based on the new end date.
                this.RaisePropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Gets or sets the st date.
        /// </summary>
        /// <value>
        /// The st date.
        /// </value>
        public DateTime StDate
        {
            get
            {
                return this.stDate;
            }
            set
            {
                /// If this task is a parent task, then it should have the minimum start time. Hence comparing the date with minimum date of its children.

                if (this.childTask != null && this.childTask.Count >= 1)
                {
                    if (value <= this.childTask.Min(s => s.stDate) && this.stDate != value)
                    {
                        this.stDate = value;
                    }
                }
                else
                {
                    this.stDate = value;
                }

                this.RaisePropertyChanged("StDate");

                // Duration chagned is invoked to notify the chagne in duration based on the new start date.
                this.RaisePropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.RaisePropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the predecessor.
        /// </summary>
        /// <value>
        /// The predecessor.
        /// </value>
        public ObservableCollection<Predecessor> Predecessor
        {
            get
            {
                return this.predecessor;
            }
            set
            {
                this.predecessor = value;
                this.RaisePropertyChanged("Predecessor");
            }
        }

        #endregion

        #region ChildTask Collection

        /// <summary>
        /// Gets or sets the child task.
        /// </summary>
        /// <value>The child task.</value>
        public ObservableCollection<Task> ChildTask
        {
            get
            {
                if (this.childTask == null)
                {
                    this.childTask = new ObservableCollection<Task>();
                    
                    // Collection changed of child tasks are hooked to listen and refresh the parent node based on the changes made in Child.
                    this.childTask.CollectionChanged += this.ChildNodesCollectionChanged;
                }

                return this.childTask;
            }
            set
            {
                this.childTask = value;
                
                //Collection changed of child tasks are hooked to listen and refresh the parent node based on the changes made in Child.
                this.childTask.CollectionChanged += this.ChildNodesCollectionChanged;
                if (value.Count > 0)
                {
                    this.childTask.ToList().ForEach(n =>
                    {
                        // To listen the changes occuring in child task.
                        n.PropertyChanged += this.ChildNodePropertyChanged;

                    });
                    this.UpdateData();
                }

                this.RaisePropertyChanged("ChildTask");
            }
        }

        /// <summary>
        /// The following does the calculations to update the Parent Task, when child collection property changes.
        /// </summary>
        /// <param name="sender">The source</param>
        /// <param name="e">Property changed event args</param>
        void ChildNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                if (e.PropertyName == "StDate" || e.PropertyName == "EndDate" || e.PropertyName == "Complete")
                {
                    this.UpdateData();
                }
            }
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        private void UpdateData()
        {
            /// Updating the start and end date based on the chagne occur in the date of child task
            this.StDate = this.childTask.Select(c => c.StDate).Min();
            this.EndDate = this.childTask.Select(c => c.EndDate).Max();
            this.Complete = this.childTask.Aggregate(0d, (cur, task) => cur + task.Complete) / this.childTask.Count;
        }

        /// <summary>
        /// Childs the nodes collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        public void ChildNodesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Task node in e.NewItems)
                {
                    node.PropertyChanged += this.ChildNodePropertyChanged;
                }
            }
            else
            {
                foreach (Task node in e.OldItems)
                {
                    node.PropertyChanged -= this.ChildNodePropertyChanged;
                }
            }

            this.UpdateData();
        }

        #endregion
    }

    #region Enum
    
    public enum RowType
    {
        Projectrow,
        SubProjectRow,
        AnalysisRow,
        ProductionRow,
        TaskRow
    }

    #endregion

}
