using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lifen
{
    internal class MainWindowViewModel
    {
        private readonly CommandBase loaded_command_;
        public ICommand loaded_command => loaded_command_;

        private readonly CommandBase closing_command_;
        public ICommand closing_command => closing_command_;


        public MainWindowViewModel()
        {            
            loaded_command_ = new CommandBase(loaded);
            closing_command_ = new CommandBase(closing);
        }

        private void loaded()
        {
            Manager.initialize();
            Manager.connect_to_db();
            Manager.execute();
        }

        private void closing()
        {
            Manager.disconnect_to_db();
        }




    }
}
