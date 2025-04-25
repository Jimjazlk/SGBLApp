namespace SGBLApp.Core.Application.Interfaces
{
    public interface IBaseService<TDto>
        where TDto : class
    {
        IEnumerable<TDto> GetAll();
        TDto GetById(int id);
        void Add(TDto dto);
        void Update(TDto dto);
        void Delete(int id);
    }

}
