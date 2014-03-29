using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Theme
    {
        #region "FIELDS"
        public static readonly Theme ThemeAcctMgmt = new Theme("Account Management");
        public static readonly Theme ThemeOrderProc = new Theme("Order Processing");
        public static readonly Theme ThemeStorefront = new Theme("Storefront");
        #endregion

        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Theme(string name)
        {
            Name = name;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddThemes(Project project)
        {
            project.Themes.Add(ThemeAcctMgmt);
            project.Themes.Add(ThemeOrderProc);
            project.Themes.Add(ThemeStorefront);
        }

        public static void SaveThemes(Project project)
        {
            IAssetType themeType = Program.MetaModel.GetAssetType("Theme");
            IAttributeDefinition themeName = themeType.GetAttributeDefinition("Name");
            IAttributeDefinition themeScope = themeType.GetAttributeDefinition("Scope");

            IList<Theme> themes = project.Themes;

            foreach (Theme theme in themes)
            {
                Asset asset = Program.Services.New(themeType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(themeName, theme.Name);
                asset.SetAttributeValue(themeScope, Oid.FromToken(project.Id, Program.MetaModel));
                Program.Services.Save(asset);
                theme.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion
    }
}
