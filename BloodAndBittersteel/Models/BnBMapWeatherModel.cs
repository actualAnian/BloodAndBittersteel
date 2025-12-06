using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Models
{
    internal class BnBMapWeatherModel : MapWeatherModel
    {
        private readonly MapWeatherModel _baseModel;

        public BnBMapWeatherModel(MapWeatherModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override CampaignTime WeatherUpdateFrequency => _baseModel.WeatherUpdateFrequency;

        public override CampaignTime WeatherUpdatePeriod => _baseModel.WeatherUpdatePeriod;

        public override AtmosphereInfo GetAtmosphereModel(CampaignVec2 position)
        {
            return _baseModel.GetAtmosphereModel(position);
        }

        public override AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos)
        {
            return _baseModel.GetInterpolatedAtmosphereState(timeOfYear, pos);
        }

        public override void GetSeasonTimeFactorOfCampaignTime(CampaignTime ct, out float timeFactorForSnow, out float timeFactorForRain, bool snapCampaignTimeToWeatherPeriod = true)
        {
            _baseModel.GetSeasonTimeFactorOfCampaignTime(ct, out timeFactorForSnow, out timeFactorForRain, snapCampaignTimeToWeatherPeriod);
            timeFactorForSnow = 0;
        }

        public override void GetSnowAndRainDataForPosition(Vec2 position, CampaignTime ct, out float snowValue, out float rainValue)
        {
            _baseModel.GetSnowAndRainDataForPosition(position, ct, out snowValue, out rainValue);
        }

        public override WeatherEventEffectOnTerrain GetWeatherEffectOnTerrainForPosition(Vec2 pos)
        {
            return _baseModel.GetWeatherEffectOnTerrainForPosition(pos);
        }

        public override WeatherEvent GetWeatherEventInPosition(Vec2 pos)
        {
            return _baseModel.GetWeatherEventInPosition(pos);
        }

        public override Vec2 GetWindForPosition(CampaignVec2 position)
        {
            return _baseModel.GetWindForPosition(position);
        }

        public override void InitializeCaches()
        {
            _baseModel.InitializeCaches();
        }
        public override WeatherEvent UpdateWeatherForPosition(CampaignVec2 position, CampaignTime ct)
        {
            return _baseModel.UpdateWeatherForPosition(position, ct);
        }
    }
}
