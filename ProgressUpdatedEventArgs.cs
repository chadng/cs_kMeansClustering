using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kmc {
    public class ProgressUpdatedEventArgs : EventArgs {
        public int Value { get; set; }
        public int Maximum { get; set; }
        public string Text { get; set; }
    }
}
