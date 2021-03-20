using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ControlLoopProcessFor3Rates.Models;

using Historian = Proficy.Historian.ClientAccess.API; // v7.1

namespace ControlLoopProcessFor3Rates
{
    class HistorianDA
    {
        private static readonly string[] _autModes = { "AUT", "CAS", "AUT IMAN", "RCAS", "AUT TRK", "CAS TRK" }; // these control loop mode values considered as AUT mode (simplified)
        //private readonly string[] _manModes = { "MAN", "MAN IMAN" };

        private const string IHistSrv = "ihistsrv";
        private const int NumOfSamples = 480; //8h long shifts in plants -> 480 mins, 1 sample every min is enough
        private const int ComparisonValue = 70; //minimum plant load 

        Historian.ServerConnection sc;

        private bool IsConnected
        {
            get { return sc.IsConnected(); }
        }

        public List<ControlLoopDataModel> QueryControlLoopsTagData(List<ModeTagsDataModel> modeTagsData, DateTime queryStart, DateTime queryEnd)
        {
            var controlLoopData = new List<ControlLoopDataModel>();

            try
            {
                Connect();

                if (!IsConnected)
                    Connect();

                foreach (var modeTagData in modeTagsData)
                {
                    Historian.DataQueryParams query =
                        new Historian.InterpolatedQuery(queryStart, queryEnd, NumOfSamples, modeTagData.TagName) { Fields = Historian.DataFields.Time | Historian.DataFields.Value | Historian.DataFields.Quality };

                    Historian.DataSet set = new Historian.DataSet();

                    query.Criteria.FilterMode = Historian.DataCriteria.FilterModeType.AfterTime; //obviously
                    query.Criteria.SamplingMode = Historian.DataCriteria.SamplingModeType.Interpolated; //we need interpolated sampling
                    query.Criteria.FilterComparisonMode = Historian.DataCriteria.FilterComparisonModeType.GreaterThanEqual;
                    query.Criteria.FilterComparisonValue = ComparisonValue; //fix 70 percent
                    query.Criteria.FilterTag = modeTagData.PlantLoadFilterTagName; //here we filter by a tag what shows the plant (where the control loop is installed) load in percentage

                    sc.IData.Query(
                        ref query,
                        out set,
                        out Historian.ItemErrors errors); //okay, about errors we don't care here... *sad smiley here*

                    if (modeTagData.ConditionFilter) //additional filtering, for example based on running signals of a motor or a value measured by a flow metering instrument out on the field
                    {
                        query.Criteria.FilterMode = Historian.DataCriteria.FilterModeType.AfterTime;
                        query.Criteria.SamplingMode = Historian.DataCriteria.SamplingModeType.Interpolated;
                        query.Criteria.FilterTag = modeTagData.ConditionFilterTagName;
                        query.Criteria.FilterComparisonMode = GetHistFilterType(modeTagData.ConditionFilterType); //filtering comparison mode stroed in the database too, depending on the filter tag
                        query.Criteria.FilterComparisonValue = modeTagData.ConditionFilterValue;

                        sc.IData.Query(ref query, out set, out errors);

                        controlLoopData.Add(ProcessDataSet(set, modeTagData.TagName));
                    }
                    else
                    {
                        controlLoopData.Add(ProcessDataSet(set, modeTagData.TagName));
                    }
                }

                return controlLoopData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tag query error: " + ex.Message);
                throw;
            }
            finally
            {
                Disconnect();
            }
        }

        private ControlLoopDataModel ProcessDataSet(Historian.DataSet set, string currentTag)
        {
            //string value, quality, timestamp;
            string value;
            float quality;
            int bad = 0, autCount = 0, manCount = 0;

            for (int i = 0; i < set.TotalSamples; i++)
            {
                value = set[currentTag].GetValue(i) == null ? " 0" : set[currentTag].GetValue(i).ToString();
                quality = set[currentTag].GetQuality(i).PercentGood();
                //timestamp = set[curr_tag].GetTime(i).ToLongTimeString(); //we dont need the timestamp 

                if (quality > 50 || value != "0")
                {
                    if (value.In(_autModes))
                    {
                        autCount++;
                    }
                    else
                    {
                        manCount++;
                    }
                }
                else
                {
                    bad++;
                }
            }

            var controlLoopData = new ControlLoopDataModel
            {
                HistTagName = currentTag,
                NumOfGoodData = 480 - bad,
                NumOfGoodQltyPercentage = bad / 480 * 100,
                NumOfGoodAUTData = autCount,
                NumOfGoodMANData = manCount,
                NumOfGoodQltyAUTPercentage = autCount / (480 - bad) * 100,
                NumOfGoodQltyMANPercentage = manCount / (480 - bad) * 100
            };

            return controlLoopData;
        }

        private protected Historian.DataCriteria.FilterComparisonModeType GetHistFilterType(int conditionFilterType)
        {
            var map = new Dictionary<int, Historian.DataCriteria.FilterComparisonModeType>()
            {
                {1, Historian.DataCriteria.FilterComparisonModeType.Equal},
                {2, Historian.DataCriteria.FilterComparisonModeType.GreaterThan},
                {3, Historian.DataCriteria.FilterComparisonModeType.LessThan},
                {4, Historian.DataCriteria.FilterComparisonModeType.GreaterThanEqual},
                {5, Historian.DataCriteria.FilterComparisonModeType.LessThanEqual},
                {6, Historian.DataCriteria.FilterComparisonModeType.NotEqual}
            };

            return map.TryGetValue(conditionFilterType, out Historian.DataCriteria.FilterComparisonModeType output) ? output : Historian.DataCriteria.FilterComparisonModeType.Equal;
        }

        private void Connect()
        {
            if (sc == null)
            {
                sc = new Historian.ServerConnection(new Historian.ConnectionProperties
                {
                    ServerHostName = IHistSrv,
                    OpenTimeout = new TimeSpan(0, 0, 10),
                    ServerCertificateValidationMode = Historian.CertificateValidationMode.None
                });
            }

            if (!sc.IsConnected())
            {
                try
                {
                    sc.Connect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error at connecting: " + ex.Message);
                    throw;
                }
            }
        }

        private void Disconnect()
        {
            if (sc.IsConnected())
            {
                try
                {
                    sc.Disconnect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error at disconnecting: " + ex.Message);
                    throw;
                }
            }

            Dispose();
        }

        private void Dispose()
        {
            ((IDisposable)sc).Dispose();
        }
    }
}
