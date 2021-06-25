using System.Collections.Generic;
using HAL.Reflection.Attributes;

namespace HAL.Documentation.Base.Monitoring
{
    public partial class Recorder
    {
        /// <summary> Get <see cref="Recorder"/> properties. </summary>
        /// <param name="recorder">Object to get properties from.</param>
        /// <param name="alias">Alias.</param>
        /// <param name="records"><see cref="IRecord"/>.</param>
        [Function("{E54DD368-289C-4C9D-BB71-7B687B6A65F6}", "Get Properties", "GetProperties", "Get Properties of a Recorder.", 0)]
        [FunctionSet("{92657D82-CA09-4938-A391-0B2B1796B0FD}", " Recorder Properties", "Recorder", "Get Recorder Properties", 1)]
        [FunctionSuite(FonctionSuite.GetProperties)]
        public static void GetProperties(Recorder recorder, out string alias, out List<IRecord> records)
        {
            records = recorder.Records;
            alias = recorder.Identity.Alias;
        }

        /// <summary> <inheritdoc cref="GroupRecords"/> </summary>
        /// <param name="recorder"> Recorder for which <see cref="IRecord"/> should be grouped.</param>
        /// <param name="recorders"><see cref="IRecord"/> grouped in sub <see cref="Recorder"/>.</param>
        [Function("{1ECE6931-09A1-41FE-8158-64CD598D66D8}", "Group Records", "Group", "Create subsets of Records", 0)]
        [FunctionSuite("{2FAFFC39-4C63-4946-B312-8A812234F85D}", "Group Records", "Group", "Create subsets of Records", 5)]
        [FunctionSubcategory(FonctionSubCategory.Monitoring)]
        public static void Group(Recorder recorder, out List<Recorder> recorders)
            => recorders = recorder.GroupRecords();


        /// <summary> Import a serialized <see cref="Recorder"/>. </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="recorder"><see cref="Recorder"/>.</param>
        [Function("{C2597AEA-4898-409C-883A-D6F790F97667}", "Import", "Import", "Import serialized Recorder.", 0)]
        [FunctionSuite("{420DFDFE-DCE7-4276-8F4E-1DE76E7A1157}", "Import", "Import", "Import serialized Recorder.", 6)]
        [FunctionSubcategory(FonctionSubCategory.Monitoring)]
        public static void Import(string filePath, out Recorder recorder) 
            => recorder = Helpers.SerializationHelpers.DeserializeCompressedJson<Recorder>(filePath);
    }
}