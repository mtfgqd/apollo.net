﻿using System;
using Com.Ctrip.Framework.Apollo.Util;
using System.Collections.Specialized;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class AppSettingsSectionBuilder : ApolloConfigurationBuilder
    {
        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (!(configSection is AppSettingsSection section)) return base.ProcessConfigurationSection(configSection);

            var appSettings = section.Settings;

            TrySetConfigUtil(appSettings);

            lock (this)
            {
                var config = GetConfig();
                foreach (var key in config.GetPropertyNames())
                {
                    var value = config.GetProperty(key, null);

                    if (!string.IsNullOrEmpty(value))
                        appSettings.Remove(key);

                    appSettings.Add(key, value);
                }
            }

            return base.ProcessConfigurationSection(configSection);
        }

        private static void TrySetConfigUtil(KeyValueConfigurationCollection appSettings)
        {
            if (Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT") == "Staging")
            {
                ConfigurationManager.AppSettings["Apollo.Env"] = "UAT";
                ConfigurationManager.AppSettings["Apollo.MetaServer"] = "http://apollo.ad.tuhu.cn:8081";

                Activator.CreateInstance<ConfigUtil>();
            }

            if (ConfigUtil.AppSettings != null) return;

            var settings = new NameValueCollection();

            foreach (var key in appSettings.AllKeys)
                settings.Add(key, appSettings[key].Value);

            ConfigUtil.AppSettings = settings;
        }
    }
}
