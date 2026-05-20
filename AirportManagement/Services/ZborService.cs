using System.Data;
using AirportManagement.Data;
using MySql.Data.MySqlClient;
using AirportManagement.Models;

namespace AirportManagement.Services
{
    public class ZborService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($@"
SELECT
  `{pk}` AS id,
  COALESCE(numar_zbor, cod) AS numar_zbor,
  COALESCE(numar_zbor, cod) AS cod,
  companie_aeriana,
  tip_zbor,
  oras_origine,
  oras_destinatie,
  data_ora_programata,
  data_ora_estimata,
  data_ora_reala,
  status,
  poarta_id,
  pista_id,
  numar_total_pasageri,
  observatii,
  creat_de,
  data_creare,
  data_modificare,
  COALESCE(oras_origine, sursa) AS sursa,
  COALESCE(oras_destinatie, destinatie) AS destinatie,
  COALESCE(data_ora_programata, plecare) AS plecare,
  COALESCE(data_ora_estimata, sosire) AS sosire
FROM zboruri", conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public bool Create(Zbor z)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(@"
INSERT INTO zboruri
(
  numar_zbor,
  companie_aeriana,
  tip_zbor,
  oras_origine,
  oras_destinatie,
  data_ora_programata,
  data_ora_estimata,
  data_ora_reala,
  status,
  poarta_id,
  pista_id,
  numar_total_pasageri,
  observatii,
  creat_de,
  data_creare,
  data_modificare,
  cod,
  sursa,
  destinatie,
  plecare,
  sosire
)
VALUES
(
  @nr,@comp,@tip,@orig,@dest,@prog,@est,@real,@st,
  @poarta,@pista,@nrpax,@obs,@creatde,@creare,@modif,
  @cod,@src,@dst,@plec,@sos
)", conn);
            cmd.Parameters.AddWithValue("@nr", z.Cod);
            cmd.Parameters.AddWithValue("@comp", z.CompanieAeriana);
            cmd.Parameters.AddWithValue("@tip", ResolveTipZbor(z));
            cmd.Parameters.AddWithValue("@orig", z.Sursa);
            cmd.Parameters.AddWithValue("@dest", z.Destinatie);
            cmd.Parameters.AddWithValue("@prog", z.Plecare);
            cmd.Parameters.AddWithValue("@est", z.Sosire);
            cmd.Parameters.AddWithValue("@real", DBNull.Value);
            cmd.Parameters.AddWithValue("@st", string.IsNullOrWhiteSpace(z.Status) ? "Programat" : z.Status);
            cmd.Parameters.AddWithValue("@poarta", DBNull.Value);
            cmd.Parameters.AddWithValue("@pista", DBNull.Value);
            cmd.Parameters.AddWithValue("@nrpax", 0);
            cmd.Parameters.AddWithValue("@obs", DBNull.Value);
            cmd.Parameters.AddWithValue("@creatde", 1);
            cmd.Parameters.AddWithValue("@creare", DateTime.Now);
            cmd.Parameters.AddWithValue("@modif", DBNull.Value);
            cmd.Parameters.AddWithValue("@cod", z.Cod);
            cmd.Parameters.AddWithValue("@src", z.Sursa);
            cmd.Parameters.AddWithValue("@dst", z.Destinatie);
            cmd.Parameters.AddWithValue("@plec", z.Plecare);
            cmd.Parameters.AddWithValue("@sos", z.Sosire);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Zbor z)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($@"
UPDATE zboruri SET
  numar_zbor=@nr,
  companie_aeriana=@comp,
  tip_zbor=@tip,
  oras_origine=@orig,
  oras_destinatie=@dest,
  data_ora_programata=@prog,
  data_ora_estimata=@est,
  data_ora_reala=@real,
  status=@st,
  poarta_id=@poarta,
  pista_id=@pista,
  numar_total_pasageri=@nrpax,
  observatii=@obs,
  creat_de=@creatde,
  data_modificare=@modif,
  cod=@cod,
  sursa=@src,
  destinatie=@dst,
  plecare=@plec,
  sosire=@sos
WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@nr", z.Cod);
            cmd.Parameters.AddWithValue("@comp", z.CompanieAeriana);
            cmd.Parameters.AddWithValue("@tip", ResolveTipZbor(z));
            cmd.Parameters.AddWithValue("@orig", z.Sursa);
            cmd.Parameters.AddWithValue("@dest", z.Destinatie);
            cmd.Parameters.AddWithValue("@prog", z.Plecare);
            cmd.Parameters.AddWithValue("@est", z.Sosire);
            cmd.Parameters.AddWithValue("@real", DBNull.Value);
            cmd.Parameters.AddWithValue("@st", string.IsNullOrWhiteSpace(z.Status) ? "Programat" : z.Status);
            cmd.Parameters.AddWithValue("@poarta", DBNull.Value);
            cmd.Parameters.AddWithValue("@pista", DBNull.Value);
            cmd.Parameters.AddWithValue("@nrpax", 0);
            cmd.Parameters.AddWithValue("@obs", DBNull.Value);
            cmd.Parameters.AddWithValue("@creatde", 1);
            cmd.Parameters.AddWithValue("@modif", DateTime.Now);
            cmd.Parameters.AddWithValue("@cod", z.Cod);
            cmd.Parameters.AddWithValue("@src", z.Sursa);
            cmd.Parameters.AddWithValue("@dst", z.Destinatie);
            cmd.Parameters.AddWithValue("@plec", z.Plecare);
            cmd.Parameters.AddWithValue("@sos", z.Sosire);
            cmd.Parameters.AddWithValue("@id", z.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($"DELETE FROM zboruri WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static string ResolveTipZbor(Zbor z)
        {
            if (!string.IsNullOrWhiteSpace(z.TipZbor))
            {
                return z.TipZbor;
            }

            if (!string.IsNullOrWhiteSpace(z.Sursa) && z.Sursa.ToLowerInvariant().Contains("cluj"))
            {
                return "plecare";
            }

            return "sosire";
        }
    }
}
