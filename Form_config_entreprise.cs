
using System;
using CFL_1.CFL_System.DB;
using CFL_1.CFL_Data;
using CFL_1.CFL_System.SqlServerOrm;
using SqlOrm;

namespace CFL_1.CFLGraphics
{
    class Form_config_entreprise : CFLForm
    {
        public Form_config_entreprise()
        {
            Init();
            buttonNew = true;
            buttonSave = true;
        }

        public override void BecomeCurrent()
        {
            
        }

        public override bool DeleteCurrent()
        {
            throw new NotImplementedException();
        }

        public override void Documents()
        {
            throw new NotImplementedException();
        }
        
        public override void GetNotification(DBNotification _notification)
        {
            //TODO
            if(!_notification.IsSentBySelf)
            {
                 
            }
        }

        public override bool NewOne()
        {
            return true;
        }

        public override bool Save()
        {
            return true;
        }

        private void LoadProject()
        {
            
        }

        private void Init()
        {
            
        }

    }
}
