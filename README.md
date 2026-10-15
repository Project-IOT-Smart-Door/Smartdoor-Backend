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
├── Data/SmartDoorDb.cs         # Akses database SQLite
├── Models/                     # Pengguna, KartuRfid, LogAkses, Pintu, HakAkses, dll.
└── Services/
    ├── AuthService.cs          # Autentikasi & validasi kartu
    └── SerialService.cs        # Komunikasi Serial dengan Arduino
database/
├── schema.sql                  # Skema database
├── seed.sql                    # Data awal
└── migrate_*.sql               # Script migrasi
```

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
