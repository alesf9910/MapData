using Example.Converters;
using Example.Models;
using Example.Models.Dto;
using MapData;
using MapData.Attributes;

namespace Example.Mappers;

[MapData]
public interface IUserMapper
{
    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    User UserFromUserRegistration(UserRegistrationDto userRegistrationDto);

    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    UserDetailDto UserDetailFromUser(User user);

    [Ignore(nameof(UserDetailDto.Birthday))]
    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    UserDetailDto UserMinimalDetailFromUser(User user);

}
