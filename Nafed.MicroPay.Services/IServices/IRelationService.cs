using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IRelationService
    {
        List<Model.Relation> GetRelationList();
        bool RelationNameExists(int? RelationID, string RelationName);
        bool RelationCodeExists(int? RelationID, string RelationCode);
        bool UpdateRelation(Model.Relation editRelationItem);
        bool InsertRelation(Model.Relation createRelation);
        Model.Relation GetRelationByID(int RelationID);
        bool Delete(int RelationID);
    }
}
