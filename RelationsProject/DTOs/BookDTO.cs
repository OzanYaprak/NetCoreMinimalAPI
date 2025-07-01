using RelationsProject.Entities;

namespace RelationsProject.DTOs
{
    public record BookDTO : BookDTOBase
    {
        //  init accessor’ı :, bir özelliğin (property) sadece nesne ilk oluşturulurken (object initialization sırasında) atanabilmesini sağlar.
        //  Yani, init ile işaretlenen bir property, nesne oluşturulduktan sonra değiştirilemez; sadece ilk atamada değer verilebilir.
        //var dto = new BookDTO { Id = 5 }; // Geçerli
        //dto.Id = 10; // Derleme hatası! (init ile tanımlandığı için)

        public int Id { get; init; }
        public Category Category { get; init; }
    }
}