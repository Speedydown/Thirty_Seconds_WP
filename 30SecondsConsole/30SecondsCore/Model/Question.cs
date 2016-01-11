using _30SecondsCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30SecondsCore.Model
{
    public class Question //: DataObject
    {
        public string Inquiry { get; private set; }
        public Languages Language { get; private set; }
        public Categories Category { get; private set; }
        public DateTime LastEdited { get; private set; }
        public bool Expired { get; private set; }

        public Question(
            int ID,
            string Inquery,
            int LanguageID,
            int CategoryID,
            DateTime LastEdited,
            bool Expired) :
        this(
            ID,
            Inquery,
            (Languages)LanguageID,
            (Categories)CategoryID,
            LastEdited,
            Expired)
        {

        }

        public Question(
            int ID,
            string Inquiry,
            Languages Language,
            Categories Category,
            DateTime LastEdited,
            bool Expired)
            : base() //TODO : ID
        {
            this.Inquiry = Inquiry;
            this.Language = Language;
            this.Category = Category;
            this.LastEdited = LastEdited;
            this.Expired = Expired;

            //if (this.ID == 0)
            //{
            //    LastEdited = TimeConverter.GetDateTime();
            //    QuestionHandler.instance.AddObject(this);
            //}
        }
    }
}
