using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;
using Android.Gms.Extensions;
using Android.Runtime;
using Android.Util;

namespace Plugin.FirebaseAuth
{
    public class UserWrapper : IUser
    {
        private readonly FirebaseUser _user;

        public UserWrapper(FirebaseUser user)
        {
            _user = user;
        }

        public static explicit operator FirebaseUser(UserWrapper wrapper)
        {
            return wrapper._user;
        }

        public string DisplayName => _user.DisplayName;

        public string Email => _user.Email;

        public bool IsAnonymous => _user.IsAnonymous;

        public string PhoneNumber => _user.PhoneNumber;

        public Uri PhotoUrl => _user.PhotoUrl != null ? new Uri(_user.PhotoUrl.ToString()) : null;

        public IEnumerable<IUserInfo> ProviderData => _user.ProviderData.Select(userInfo => new UserInfoWrapper(userInfo));

        public string ProviderId => _user.ProviderId;

        public string Uid => _user.Uid;

        public bool IsEmailVerified => _user.IsEmailVerified;

        public IUserMetadata Metadata => _user.Metadata != null ? new UserMetadataWrapper(_user.Metadata) : null;

        public async Task DeleteAsync()
        {
            try
            {
                await _user.DeleteAsync().ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task<string> GetIdTokenAsync(bool forceRefresh)
        {
            try
            {
                var result = await _user.GetIdToken(forceRefresh).AsAsync<GetTokenResult>().ConfigureAwait(false);
                return result.Token;
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        private object ConvertClaimsValue(Java.Lang.Object profileValue)
        {
            if (profileValue == null)
                return null;

            switch (profileValue)
            {
                case Java.Lang.Boolean @boolean:
                    return (bool)@boolean;
                case Java.Lang.Short @short:
                    return (short)@short;
                case Java.Lang.Integer @integer:
                    return (int)@integer;
                case Java.Lang.Long @long:
                    return (long)@long;
                case Java.Lang.Float @float:
                    return (float)@float;
                case Java.Lang.Double @double:
                    return (double)@double;
                case Java.Lang.Number @number:
                    return (double)@number;
                case Java.Lang.String @string:
                    return profileValue.ToString();
                case JavaList javaList:
                    {
                        var list = new List<object>();
                        foreach (var data in javaList)
                        {
                            var value = data;
                            if (value is Java.Lang.Object javaObject)
                            {
                                value = ConvertClaimsValue(javaObject);
                            }
                            list.Add(value);
                        }
                        return list;
                    }
                case JavaDictionary javaDictionary:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var key in javaDictionary.Keys)
                        {
                            var value = javaDictionary[key];
                            if (value is Java.Lang.Object javaObject)
                            {
                                value = ConvertClaimsValue(javaObject);
                            }
                            dict[key.ToString()] = value;
                        }
                        return dict;
                    }
                case ArrayMap arrayMap:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var key in arrayMap.KeySet())
                        {
                            dict[key.ToString()] = ConvertClaimsValue(arrayMap.Get(new Java.Lang.String(key.ToString())));
                        }
                        return dict;
                    }
                default:
                    if (profileValue.ToString() == "null")
                    {
                        return null;
                    }
                    return profileValue;
            }
        }

        public async Task<IDictionary<string, object>> GetClaimsAsync(bool forceRefresh)
        {
            try
            {
                var result = await _user.GetIdToken(forceRefresh).AsAsync<GetTokenResult>().ConfigureAwait(false);
                return result.Claims.ToDictionary(x => x.Key, elementSelector: x => ConvertClaimsValue(x.Value));
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task<IAuthResult> LinkWithCredentialAsync(IAuthCredential credential)
        {
            try
            {
                var wrapper = (AuthCredentialWrapper)credential;
                var result = await _user.LinkWithCredentialAsync((AuthCredential)wrapper).ConfigureAwait(false);
                return new AuthResultWrapper(result);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task<IAuthResult> ReauthenticateAndRetrieveDataAsync(IAuthCredential credential)
        {
            try
            {
                var wrapper = (AuthCredentialWrapper)credential;
                var result = await _user.ReauthenticateAndRetrieveDataAsync((AuthCredential)wrapper).ConfigureAwait(false);
                return new AuthResultWrapper(result);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task ReloadAsync()
        {
            try
            {
                await _user.ReloadAsync().ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task SendEmailVerificationAsync()
        {
            try
            {
                await _user.SendEmailVerificationAsync(null).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task SendEmailVerificationAsync(ActionCodeSettings actionCodeSettings)
        {
            try
            {
                await _user.SendEmailVerificationAsync(actionCodeSettings.ToNative()).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task<IUser> UnlinkAsync(string providerId)
        {
            try
            {
                var result = await _user.UnlinkAsync(providerId).ConfigureAwait(false);
                return new UserWrapper(result.User);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task UpdateEmailAsync(string email)
        {
            try
            {
                await _user.UpdateEmailAsync(email).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task UpdatePasswordAsync(string password)
        {
            try
            {
                await _user.UpdatePasswordAsync(password).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task UpdatePhoneNumberAsync(IPhoneAuthCredential credential)
        {
            try
            {
                var wrapper = (PhoneAuthCredentialWrapper)credential;
                await _user.UpdatePhoneNumberAsync((PhoneAuthCredential)wrapper).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }

        public async Task UpdateProfileAsync(UserProfileChangeRequest request)
        {
            try
            {
                var builder = new Firebase.Auth.UserProfileChangeRequest.Builder();

                if (request.IsDisplayNameChanged)
                {
                    builder.SetDisplayName(request.DisplayName);
                }
                if (request.IsPhotoUrlChanged)
                {
                    var uri = request.PhotoUrl != null ? Android.Net.Uri.Parse(request.PhotoUrl.ToString()) : null;
                    builder.SetPhotoUri(uri);
                }

                await _user.UpdateProfileAsync(builder.Build()).ConfigureAwait(false);
            }
            catch (FirebaseException e)
            {
                throw ExceptionMapper.Map(e);
            }
        }
    }
}
