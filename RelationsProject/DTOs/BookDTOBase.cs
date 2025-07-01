using System.ComponentModel.DataAnnotations;

namespace RelationsProject.DTOs
{
    // 'abstract': Bu anahtar kelime, sınıfın veya record'un doğrudan örneklendirilemeyeceğini belirtir.
    // Yani, 'abstract' olarak işaretlenen bir sınıftan veya record'dan nesne oluşturulamaz.
    // Sadece kalıtım yoluyla başka bir sınıf veya record tarafından genişletilebilir.

    // 'record': Bu anahtar kelime, C#'ta öncelikli olarak veri taşıma amacıyla kullanılan referans türlerini tanımlar.
    // 'record' türleri, değer tabanlı eşitlik ve değişmezlik (immutability) gibi özellikler sunar.

    public abstract record BookDTOBase
    {
        [MinLength(2, ErrorMessage = "Min. lenght must be 2")]
        [MaxLength(250, ErrorMessage = "Max. lenght must be 250")]
        public String Title { get; init; }

        [Range(10, 100)]
        public Decimal Price { get; init; }
    }
}