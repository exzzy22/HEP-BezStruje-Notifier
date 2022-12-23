#  HEP BezStruje Notifier

## Opis

<p>Parser za https://www.hep.hr/ods/bez-struje/19 koji povlači informacije o prekidu isporuke struje za postavljenu lokaciju za trenutni dan te sljedeća dva i obavještava korisnika emailom u slučaju nestanke struje za postavljenu lokaciju </p>

## Kako koristit

<p>Potrebno je popunit appsettings.json datoteku sa ispravnim podatcima</p>

```json
{
  "Info": {
    "Place": "Mjesto kako je napisano na https://www.hep.hr/ods/bez-struje/ ",
    "Street": "Ulica kako je napisano na https://www.hep.hr/ods/bez-struje/",
    "BaseUrl": "Primjer https://www.hep.hr/ods/bez-struje/19?dp=zagreb&el=ZG&datum="
  },
  "Email": {
    "Name": "Ime koje se prikazuje u emailu koji dobijete",
    "Address": "Email adresa emaila s kojega se šalje",
    "Host": "smtp host",
    "Port": 587,
    "Username": "username",
    "Password": "password"
  },
  "EmailList": {
    "Emails": [ "email@gmail.com","example@mail.com" ]
  }
}
```
<p>BaseUrl se može dobiti tako da se ode na https://www.hep.hr/ods/bez-struje/19 izabere distribucijsko područje i pogon.Nakon toga je potrebno kliknuti na neki od datuma da se učitaju parametri u url te se uzima dio cijeli url bez datuma na kraju, primjer za Elektru Zagreb i pogon Elektra Zagreb https://www.hep.hr/ods/bez-struje/19?dp=zagreb&el=ZG&datum=</p>

### Email konfiguracija

<p>Pod Email konfiguraciju se upisuju SMTP podatci za email, osobno koristim outlook budući da je jedan od rijetkih popularnih email servisa koji ima tu mogućnost, pod email listu se uspiju email adrese na koje želite poslati email u slučaju nestanka struje</p>

<p>S obzirom na to da se radi o konzolnoj aplikaciji bez ikakvog background servisa aplikacija će se samo jednom izvršiti zatvoriti. Tako da je potrebno podesiti nekakav scheduling program koji će periodički pokrenuti aplikaciju kao npr. Windows Task Scheduler ili Cron. Osobno koristim Cron job koji se izvršava jednom dnevno</p>
