using System.Text.Json;

namespace SmartDoor.Backend.Data;

// Konfigurasi bersama backend + frontend. Satu-satunya tempat connection string
// dan port serial didefinisikan, supaya kedua repo tidak bisa berbeda pendapat.
//
// Urutan pembacaan (yang pertama menang):
//   1. Environment variable  SMARTDOOR_CONNECTION / SMARTDOOR_SERIAL_PORT
//   2. File smartdoor.config.json di samping exe
//   3. Default di bawah (LocalDB + COM3)
public sealed record SmartDoorConfig(string ConnectionString, string SerialPort, int IdPintu)
{
    // SQL Server LocalDB — ikut terpasang bersama Visual Studio.
    public const string LocalDb =
        @"Server=(localdb)\MSSQLLocalDB;Database=SmartDoor;Trusted_Connection=True;TrustServerCertificate=True";

    // SQL Server Express — sama dengan contoh di database/schema.sql.
    public const string Express =
        @"Server=localhost\SQLEXPRESS;Database=SmartDoor;Trusted_Connection=True;TrustServerCertificate=True";

    public const string EnvConnection = "SMARTDOOR_CONNECTION";
    public const string EnvSerialPort = "SMARTDOOR_SERIAL_PORT";
    public const string FileName = "smartdoor.config.json";

    // True kalau memakai LocalDB (dipakai untuk pesan diagnosa di UI).
    public bool PakaiLocalDb =>
        ConnectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);

    // Ambil nama server dari connection string, untuk ditampilkan di layar admin.
    public string NamaServer
    {
        get
        {
            foreach (var bagian in ConnectionString.Split(';'))
            {
                var i = bagian.IndexOf('=');
                if (i <= 0) continue;
                var kunci = bagian[..i].Trim();
                if (kunci.Equals("Server", StringComparison.OrdinalIgnoreCase)
                    || kunci.Equals("Data Source", StringComparison.OrdinalIgnoreCase))
                    return bagian[(i + 1)..].Trim();
            }
            return "(tidak diketahui)";
        }
    }

    // Baca konfigurasi dari basePath (default: folder exe aplikasi).
    // Tidak pernah melempar exception — file hilang/rusak → pakai default,
    // supaya aplikasi tetap jalan dan error koneksi terlihat di UI, bukan crash.
    public static SmartDoorConfig Load(string? basePath = null)
    {
        var isi = BacaFile(Path.Combine(basePath ?? AppContext.BaseDirectory, FileName));

        var cs = Pertama(
            Environment.GetEnvironmentVariable(EnvConnection),
            isi?.ConnectionString,
            LocalDb);

        var port = Pertama(
            Environment.GetEnvironmentVariable(EnvSerialPort),
            isi?.SerialPort,
            "COM3");

        var pintu = 1;
        if (isi?.IdPintu is int p && p > 0) pintu = p;

        return new SmartDoorConfig(cs, port, pintu);
    }

    // Pilih nilai pertama yang tidak kosong.
    private static string Pertama(params string?[] nilai)
    {
        foreach (var v in nilai)
            if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
        return "";
    }

    private static readonly JsonSerializerOptions OpsiJson = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private static IsiConfig? BacaFile(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;
            return JsonSerializer.Deserialize<IsiConfig>(File.ReadAllText(path), OpsiJson);
        }
        catch
        {
            return null; // file rusak → pemanggil memakai default
        }
    }
}

// Bentuk smartdoor.config.json. Internal (bukan private) supaya serializer
// bisa membuat instance-nya.
internal sealed class IsiConfig
{
    public string? ConnectionString { get; set; }
    public string? SerialPort { get; set; }
    public int? IdPintu { get; set; }
}
