namespace Flex.Entity.Api.CustomFilters.Swagger
{
   /// <summary>
   /// 
   /// </summary>
   public interface IProvideExample
   {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="mediaType"></param>
       /// <returns></returns>
       object GetExample(string mediaType = "application/json");
   }
}
