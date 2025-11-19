using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace FamilyTreeApp.UI.Forms
{
    public partial class MapForm : Form
    {
        private FamilyTreeService familyService;
        private WebView2 webView;

        public MapForm(FamilyTreeService service)
        {
            this.familyService = service;
            InitializeComponent();
            InitializeWebView();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(1200, 800);
            this.Text = "Mapa de ubicaciones";
            this.StartPosition = FormStartPosition.CenterParent;

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);
            this.ResumeLayout(false);
        }

        private async void InitializeWebView()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                string htmlContent = GenerateMapHtml();
                webView.NavigateToString(htmlContent);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el mapa: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateMapHtml()
        {
            var members = familyService.GetAllMembers();
            var locationsJson = JsonSerializer.Serialize(members.Select(m => new
            {
                id = m.Id,
                name = m.FullName,
                lat = m.Residence.Latitude,
                lon = m.Residence.Longitude,
                photo = ConvertImageToBase64(m.PhotoPath),
                age = m.Age,
                isAlive = m.IsAlive
            }));

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Mapa Familiar</title>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <style>
        body {{ margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
        #map {{ width: 100%; height: 100vh; }}
        .custom-marker-container {{ width: 80px !important; height: 80px !important; margin-left: -40px !important; margin-top: -40px !important; }}
        .custom-marker {{ width: 80px; height: 80px; border-radius: 50%; border: 5px solid #0078d4; background: white; display: flex; align-items: center; justify-content: center; cursor: pointer; box-shadow: 0 4px 12px rgba(0,0,0,0.4); position: relative; transition: all 0.3s; }}
        .custom-marker:hover {{ border-color: #ff6b6b; transform: scale(1.2); box-shadow: 0 6px 20px rgba(0,0,0,0.5); }}
        .marker-photo {{ width: 70px; height: 70px; border-radius: 50%; object-fit: cover; }}
        .marker-initial {{ font-size: 36px; font-weight: bold; color: #0078d4; }}
        .marker-pulse {{ position: absolute; width: 80px; height: 80px; border-radius: 50%; background: rgba(0, 120, 212, 0.3); animation: pulse 2s infinite; }}
        @keyframes pulse {{ 0% {{ transform: scale(1); opacity: 1; }} 100% {{ transform: scale(1.5); opacity: 0; }} }}
        .distance-info {{ background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.2); max-width: 300px; }}
        .distance-info h3 {{ margin: 0 0 10px 0; color: #0078d4; }}
        .distance-item {{ padding: 5px 0; border-bottom: 1px solid #eee; }}
        .distance-item:last-child {{ border-bottom: none; }}
        .info-panel {{ position: absolute; top: 10px; right: 10px; background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.3); z-index: 1000; max-width: 250px; }}
        .info-panel h4 {{ margin: 0 0 10px 0; color: #0078d4; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <div class='info-panel'>
        <h4>Mapa familiar</h4>
        <p style='margin: 5px 0; font-size: 14px;'>
            <strong>Total:</strong> <span id='total-markers'>0</span> personas<br>
            <strong>Hacé click</strong> en un marcador para ver distancias :P
        </p>
    </div>
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <script>
        const locations = {locationsJson};
        let map;
        let markers = {{}};
        let distanceLines = [];

        function initMap() {{
            document.getElementById('total-markers').textContent = locations.length;
            let centerLat, centerLon, zoom;
            
            if (locations.length === 0) {{
                centerLat = 9.9281; centerLon = -84.0907; zoom = 8;
                alert('No hay UBICACIONES SJAIF0AH');
            }} else if (locations.length === 1) {{
                centerLat = locations[0].lat; centerLon = locations[0].lon; zoom = 12;
            }} else {{
                centerLat = locations.reduce(function(sum, loc) {{ return sum + loc.lat; }}, 0) / locations.length;
                centerLon = locations.reduce(function(sum, loc) {{ return sum + loc.lon; }}, 0) / locations.length;
                zoom = 6;
            }}
            
            map = L.map('map').setView([centerLat, centerLon], zoom);
            L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{ attribution: '© OpenStreetMap contributors', maxZoom: 19 }}).addTo(map);
            
            locations.forEach(function(location, index) {{
                console.log('Agregando marcador ' + (index + 1) + ':', location.name, location.lat, location.lon);
                addMarker(location);
            }});
            
            if (locations.length > 1) {{
                const bounds = L.latLngBounds(locations.map(function(l) {{ return [l.lat, l.lon]; }}));
                map.fitBounds(bounds, {{ padding: [50, 50] }});
            }}
        }}

        function addMarker(location) {{
            const initial = location.name.charAt(0).toUpperCase();
            const photoHtml = location.photo && location.photo !== '' ? '<img src=""' + location.photo + '"" class=""marker-photo"" onerror=""this.style.display=\'none\'; this.nextElementSibling.style.display=\'block\';"" /><div class=""marker-initial"" style=""display:none;"">' + initial + '</div>' : '<div class=""marker-initial"">' + initial + '</div>';
            const markerHtml = '<div class=""custom-marker""><div class=""marker-pulse""></div>' + photoHtml + '</div>';
            const icon = L.divIcon({{ className: 'custom-marker-container', html: markerHtml, iconSize: [80, 80], iconAnchor: [40, 40], popupAnchor: [0, -40] }});
            const marker = L.marker([location.lat, location.lon], {{ icon: icon }}).addTo(map).bindPopup('<div style=""text-align: center; min-width: 150px;""><h3 style=""margin: 0 0 10px 0; color: #0078d4;"">' + location.name + '</h3><p style=""margin: 5px 0;""><strong>Edad:</strong> ' + location.age + ' años<br><strong>Estado:</strong> ' + (location.isAlive ? 'Vivo' : 'Fallecido') + '</p><button onclick=""window.showDistances(\'' + location.id + '\')"" style=""padding: 8px 16px; background: #0078d4; color: white; border: none; border-radius: 4px; cursor: pointer; margin-top: 10px;"">Ver distancias</button></div>', {{ maxWidth: 300 }});
            marker.on('click', function() {{ console.log('Click en marcador:', location.name); }});
            markers[location.id] = marker;
        }}

        window.showDistances = function(personId) {{
            console.log(personId);
            distanceLines.forEach(function(line) {{ map.removeLayer(line); }});
            distanceLines = [];
            window.chrome.webview.postMessage({{ action: 'getDistances', personId: personId }});
        }};

        function displayDistances(personId, distances) {{
            const selectedLocation = locations.find(function(l) {{ return l.id === personId; }});
            if (!selectedLocation) {{ console.error('No se encontró la ubicación:', personId); return; }}
            console.log(selectedLocation.name);
            
            let html = '<div class=""distance-info""><h3>Distancias desde ' + selectedLocation.name + '</h3>';
            let hasDistances = false;
            
            Object.keys(distances).forEach(function(targetId) {{
                const targetLocation = locations.find(function(l) {{ return l.id === targetId; }});
                if (targetLocation && targetId !== personId) {{
                    const distance = distances[targetId];
                    html += '<div class=""distance-item""><b>' + targetLocation.name + ':</b> ' + distance.toFixed(2) + ' km</div>';
                    const line = L.polyline([[selectedLocation.lat, selectedLocation.lon], [targetLocation.lat, targetLocation.lon]], {{ color: '#ff6b6b', weight: 3, dashArray: '10, 10', opacity: 0.8 }}).addTo(map);
                    distanceLines.push(line);
                    hasDistances = true;
                }}
            }});
            
            if (!hasDistances) {{ html += '<p style=""color: #999;"">No hay otras ubicaciones para compararxd</p>'; }}
            html += '</div>';
            markers[personId].bindPopup(html, {{ maxWidth: 350 }}).openPopup();
        }}

        window.chrome.webview.addEventListener('message', function(event) {{
            console.log(event.data);
            if (event.data.action === 'displayDistances') {{ displayDistances(event.data.personId, event.data.distances); }}
        }});

        if (document.readyState === 'loading') {{ document.addEventListener('DOMContentLoaded', initMap); }} else {{ initMap(); }}
    </script>
</body>
</html>";
        }

        // Convierte a base64 para q la lea el html
        private string ConvertImageToBase64(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
            {
                return string.Empty;
            }

            try
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                string base64String = Convert.ToBase64String(imageBytes);
                string extension = System.IO.Path.GetExtension(imagePath).ToLower();
                string mimeType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".bmp" => "image/bmp",
                    ".gif" => "image/gif",
                    _ => "image/jpeg"
                };

                return $"data:{mimeType};base64,{base64String}";
            }
            catch (Exception ex)
            {
                return string.Empty; //todo salio mal!!! D:
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(e.WebMessageAsJson);

                if (message.TryGetValue("action", out var action) &&
                    action.GetString() == "getDistances")
                {
                    if (message.TryGetValue("personId", out var personIdElement))
                    {
                        string personId = personIdElement.GetString();
                        var distances = familyService.GetDistancesFrom(personId);

                        var response = new
                        {
                            action = "displayDistances",
                            personId = personId,
                            distances = distances
                        };

                        string jsonResponse = JsonSerializer.Serialize(response);
                        webView.CoreWebView2.PostWebMessageAsJson(jsonResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error:{ex.Message}","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}