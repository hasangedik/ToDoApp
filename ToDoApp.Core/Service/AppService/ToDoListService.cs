using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ToDoApp.Core.Persistence;
using ToDoApp.Entity.Model;

namespace ToDoApp.Core.Service.AppService
{
    public class ToDoListService : AppServiceBase<ToDoList>
    {
        private readonly IUnitOfWork<DbContext> _unitOfWork;
        private readonly IToDoListRepository _toDoListRepository;

        public ToDoListService(IToDoListRepository toDoListRepository, IUnitOfWork<DbContext> unitOfWork)
        {
            _toDoListRepository = toDoListRepository;
            _unitOfWork = unitOfWork;
        }

        public List<ToDoList> GetAll()
        {
            return _toDoListRepository.GetAll().ToList();
        }

        public ToDoList Save(ToDoList toDoList)
        {
            if (toDoList.Id != default(int))
                return Update(toDoList);

            var entity = _toDoListRepository.Save(toDoList);
            _unitOfWork.Commit();
            return entity;
        }

        private ToDoList Update(ToDoList toDoList)
        {
            var dbEntity = _toDoListRepository.Get(toDoList.Id);
            SetUpdateFields(dbEntity, ref toDoList);
            _toDoListRepository.Update(dbEntity);
            _unitOfWork.Commit();
            return dbEntity;
        }

        public ToDoList Get(int id)
        {
            return _toDoListRepository.Get(id);
        }

        protected override void SetUpdateFields(ToDoList dbEntity, ref ToDoList entity)
        {
            entity = SetEntityFields(dbEntity, entity,
                ToDoList.Properties.Title,
                ToDoList.Properties.IsChecked,
                ToDoList.Properties.ModifiedBy,
                ToDoList.Properties.ModifiedOn
            );
        }
    }
}
