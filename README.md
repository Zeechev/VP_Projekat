# GalaxyPPG 

## Opis projekta

Projekat implementira WCF sistem za razmenu i obradu PPG/HRV podataka sa nosivih uređaja.

Klijent učitava PPG.csv, HR.csv i ACC.csv podatke iz GalaxyPPG dataset-a i šalje ih WCF servisu korišćenjem netTcpBinding komunikacije.

Servis validira podatke, čuva ih na disku i vrši analizu signala i warning događaja.

---

## Arhitektura sistema

![GalaxyPPG Arhitektura](arhitektura.drawio.png)

## Implementirane funkcionalnosti

### WCF komunikacija
- `StartSession`
- `PushSample`
- `EndSession`

### Obrada podataka
- parsiranje `PPG.csv`
- parsiranje `HR.csv`
- parsiranje `ACC.csv`
- rad sa `InvariantCulture`
- mapiranje `NaN` vrednosti na `null`

### Validacija i fault handling
- `FaultException`
- `ValidationFault`
- `DataFormatFault`

### Event sistem
Implementirani događaji:
- `OnTransferStarted`
- `OnSampleReceived`
- `OnTransferCompleted`
- `OnWarningRaised`

### Analitika
Sistem vrši:
- analizu HeartRate vrednosti,
- analizu IBI odstupanja,
- analizu intenziteta pokreta,
- analizu kvaliteta PPG signala.

### Warning sistem
Implementirana upozorenja:
- `HrOutOfRangeWarning`
- `IbiSpikeWarning`
- `ExcessiveMotionWarning`
- `WeakPpgWarning`

---

## Pokretanje aplikacije

Potrebno je prvo pokrenuti:

```text
GalaxyPPG.Host
```

Nakon toga pokrenuti:

```text
GalaxyPPG.Client
```

Klijent zatim traži:
1. putanju do dataset foldera,
2. ID učesnika.

Primer:

```text
C:\GalaxyPPG\Dataset
P02
```

---

## Dataset

Dataset nije dodat u repository zbog veličine.

Korišćen dataset:

```text
https://zenodo.org/records/14635823
```

Primer strukture dataset-a:

```text
Dataset/
└── P02/
    └── GalaxyWatch/
        ├── PPG.csv
        ├── HR.csv
        └── ACC.csv
```

---

## Generisani fajlovi

Server automatski generiše:

```text
session.csv
rejects.csv
```

Klijent generiše:

```text
rejected_client.csv
```

Primer izlazne strukture:

```text
Data/
└── P02/
    └── GalaxyWatch/
        └── yyyy-MM-dd/
            ├── session.csv
            └── rejects.csv
```

---

## Korišćene tehnologije

- C#
- .NET Framework
- WCF
- netTcpBinding
- CSV parsing
- FileStream
- StreamWriter
- IDisposable pattern

---

## Primer rada sistema

```text
Klijent povezan na servis.
Session started.
Participant: P02
Device: GalaxyWatch

WARNING: HrOutOfRangeWarning | HeartRate je van dozvoljenog opsega.
WARNING: ExcessiveMotionWarning | Detektovan prekomeran pokret.

Primljeno uzoraka: 100

Prenos zavrsen na klijentu.
Ukupno poslato uzoraka: 3781
```

---

## Autor

Aleksandar Zečev — PR133/2022