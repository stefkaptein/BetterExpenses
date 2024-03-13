using BlazorBootstrap;

namespace BetterExpenses.Web.Models.Toasts.Settings;

public static class UserSettingsToasts
{
    public static ToastMessage SettingsSavedToast => new ToastMessage(ToastType.Success, IconName.Check, "Success", "Settings saved");

}