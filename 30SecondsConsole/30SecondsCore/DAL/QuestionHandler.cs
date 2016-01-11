using _30SecondsCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30SecondsCore.DAL
{
    public class QuestionHandler //: DataHandler
    {
        //Letop Gaat waarschijnlijk niet werken vanwege de enums
        //private static readonly Field InquiryField = new Field("Inquiry", typeof(string), 100);
        //private static readonly Field LanguageField = new Field("Language", typeof(int), 1);
        //private static readonly Field CategoryField = new Field("Category", typeof(int), 1);
        //private static readonly Field LastEditedField = new Field("LastEdited", typeof(DateTime), 1);
        //private static readonly Field ExpiredField = new Field("Expired", typeof(bool), 1);

        public static readonly QuestionHandler instance = new QuestionHandler();

        //private QuestionHandler() : base(
        //    "Questions",
        //    new Field[] {
        //        InquiryField,
        //        LanguageField,
        //        CategoryField,
        //        LastEditedField,
        //        ExpiredField
        //    },
        //    typeof(Question))
        //{

        //}

        //public Question[] GetQuestionsByLanguage()
        //{

        //}
    }
}
