//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Devices.WiFi;

namespace WiFiScan
{
    public sealed partial class Scenario1_CachedNetworks : Page
    {
        private IReadOnlyList<WiFiAdapter> wiFiAdapters;

        public Scenario1_CachedNetworks()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // RequestAccessAsync must have been called at least once by the app before using the API
            // Calling it multiple times is fine but not necessary
            // RequestAccessAsync must be called from the UI thread
            var result = await WiFiAdapter.RequestAccessAsync();
            if (result != WiFiAccessStatus.Allowed)
            {
                ScenarioOutput.Text = "Access denied";
            }
            else
            {
                wiFiAdapters = await WiFiAdapter.FindAllAdaptersAsync();
                int index = 0;
                foreach (var adapter in wiFiAdapters)
                {
                    var button = new Button();
                    button.Tag = index;
                    button.Content = String.Format("Adapter #{0}", index++);
                    button.Click += Button_Click;
                    Buttons.Children.Add(button);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var index = (int)button.Tag;

            // Accessing the NetworkReport property from the WiFiAdapter does not trigger any scans.
            // The NetworkReport will have the report generated by the last scan on the system; the
            // scan need not be generated by the app.
            DisplayNetworkReport(wiFiAdapters[index].NetworkReport);
        }

        private void DisplayNetworkReport(WiFiNetworkReport report)
        {
            var message = string.Format("Network Report Timestamp: {0}", report.Timestamp);
            foreach (var network in report.AvailableNetworks)
            {
                message += string.Format("\nNetworkName: {0}, BSSID: {1}, RSSI: {2}dBm, Channel Frequency: {3}kHz",
                    network.Ssid, network.Bssid, network.NetworkRssiInDecibelMilliwatts, network.ChannelCenterFrequencyInKilohertz);
            }
            ScenarioOutput.Text = message;
        }
    }
}