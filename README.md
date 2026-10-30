# Smartdoor-Backend

Backend sistem **Project IOT Smart Door** — pintu pintar berbasis kartu RFID (tugas kelompok mata kuliah Internet of Things, Kelompok 4). Berisi class library C#/.NET dan skrip database.

> Bagian dari organisasi [Project-IOT-Smart-Door](https://github.com/Project-IOT-Smart-Door):
> - [Smartdoor-Backend](https://github.com/Project-IOT-Smart-Door/Smartdoor-Backend) (repo ini)
> - [Smartdoor-Frontend](https://github.com/Project-IOT-Smart-Door/Smartdoor-Frontend) — aplikasi desktop WPF
> - [Smartdoor-Firmware](https://github.com/Project-IOT-Smart-Door/Smartdoor-Firmware) — kode Arduino

## Cara kerja singkat
1. Kartu RFID di-tap pada modul RC522.
2. Arduino Uno mengirim UID ke komputer lewat Serial (`UID:xxxx`).
3. Aplikasi desktop memanggil backend untuk mencocokkan UID dengan database dan membalas `OPEN` atau `DENY`.
4. Setiap tap tercatat di tabel `LOG_AKSES`.

## Isi repository
```
SmartDoor.Backend.slnx          # Solution .NET
src/SmartDoor.Backend/
├── Data/SmartDoorDb.cs         # Akses database SQL Server via Dapper
├── Data/SmartDoorConfig.cs     # Konfigurasi bersama backend + frontend
├── Models/                     # Pengguna, KartuRfid, LogAkses, Pintu, HakAkses, dll.
└── Services/
    ├── AuthService.cs          # Autentikasi & validasi kartu
    └── SerialService.cs        # Komunikasi Serial dengan Arduino
database/
├── schema.sql                  # Skema database
├── seed.sql                    # Data awal
└── migrate_*.sql               # Script migrasi
smartdoor.config.example.json   # Contoh konfigurasi (disalin ke frontend)
```

## Integrasi dengan Frontend

Frontend ([Smartdoor-Frontend](https://github.com/Project-IOT-Smart-Door/Smartdoor-Frontend)) memakai
class library ini lewat `ProjectReference`, jadi **kedua repo harus di-clone bersebelahan**
dengan nama folder persis seperti ini:

```
Project-IOT/
├── Smartdoor-Backend/          # repo ini
│   └── src/SmartDoor.Backend/SmartDoor.Backend.csproj
└── Smartdoor-Frontend/
    └── src/SmartDoor.App/SmartDoor.App.csproj   # reference: ..\..\..\Smartdoor-Backend\...
```

Kalau susunan folder berbeda, sesuaikan `ProjectReference` di `SmartDoor.App.csproj`.

Satu-satunya tempat connection string dan port serial didefinisikan adalah
`SmartDoorConfig` (`Data/SmartDoorConfig.cs`). Urutan pembacaan:

1. Environment variable `SMARTDOOR_CONNECTION` / `SMARTDOOR_SERIAL_PORT`
2. File `smartdoor.config.json` di samping exe aplikasi
3. Default: LocalDB + COM3

Alur data: `Arduino → SerialService → AuthService → SmartDoorDb → SQL Server`,
lalu balasan `OPEN`/`DENY` dikirim balik ke Arduino.

## Menjalankan
```bash
dotnet build SmartDoor.Backend.slnx
```
Class library ini di-reference oleh aplikasi [Smartdoor-Frontend](https://github.com/Project-IOT-Smart-Door/Smartdoor-Frontend) dan menjalankan logika auth serta komunikasi serial.

## Perangkat
Arduino Uno, modul RFID RC522, buzzer, LED merah/hijau, relay 1 channel, solenoid door lock (catu daya terpisah + dioda flyback).

## Tim
| Nama | Peran |
|---|---|
| Bayu Putra Setia Eka Premana | Project Manager & Embedded Developer |
| Aria Julianto Wasnita | Hardware / IoT Engineer |
| Ibnu Fatur Rahman | Frontend Developer |
| Muhamad Falzen Ridwan | Backend Developer & Database Admin |
| Suripto | DevOps / Deployment Engineer |
| Wahyu Pahrudin | QA Tester & Technical Writer |

## Aturan kerja
- Jangan commit hasil build (`*.exe`, `bin/`, `obj/`) atau password. File `.gitignore` sudah menanganinya.
- Setiap anggota bekerja di branch sendiri, lalu membuat Pull Request ke `main`.
- Pesan commit singkat dan jelas, misalnya `database: tambah default field`.
