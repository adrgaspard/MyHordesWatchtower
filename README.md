# MyHordesWatchtower

The aim of this project is to automate and standardize information gathering in the [MyHordes](https://myhordes.eu) game.
This process is not usually carried out because it is redundant and time-consuming, but it brings a significant advantage: in [MyHordes](https://myhordes.eu), **knowledge is power**.

## How it works

MyHordesWatchtower uses [Playwright](https://playwright.dev/), an automatable and scriptable browser, to collect information on all citizens. The collection procedure is as follows:
- Log on to [MyHordes](https://myhordes.eu) using credentials or cookies.
- Go to the page listing the town's citizens.
- For each citizen, collect data by going to his/her personal page, before returning to the citizens page.

All this is then stored in a database.
The ultimate goal is to be able to launch this data collection several times a day, so as to be able to monitor what's happening in the entire game.
The completeness and temporality of the information on each player makes it possible to trace who has done what over the last few hours/day.
This makes it possible to track down a mysterious thief or ghoul, or to prepare before committing such an act.

### Collected data

For each batch, the following data is collected from all citizens:

- [MyHordes](https://myhordes.eu) identity
	- **Account unique identifier**
	- **Pseudo**
- Job
	- **Profession** (guard, hermit, technician, tamer, etc.)
	- **Chaman** (whether it is or not)
- Presence
	- **Activity stars** (with "clairvoyance" capacity)
	- **Well uses**
	- **Last connection** date & time (useful for tracking a ghoul)
- Location
	- Whether it is **in town**
	- **Square position** (if not in town)
- State
	- **Death cause** (if dead)
	- Whether it is **injured**
	- Whether it is **infected**
	- Whether it is **terrified**
	- Whether it is a **drug addict**
	- Whether it is **dehydrated**
- Justice
	- Whether it is **banned**
	- **Charges** count
- Constructions registry
	- **Home level**
	- All **constructions done in the home**
- Storage
	- Wether the personal chest is **visible**
	- All **items** in personal chest
- Score
	- Home **defense**
	- Home **decoration**

More data will be collected in the future, such as :

- Town activity main register
- Bank storage
- Night watchmen

### Queries

There are a huge number of possible database queries.
We can imagine queries listing all citizens with a particular characteristic (e.g. drug addicts), or a query on the evolution of a citizen's positions in the desert).
If you want a particular query to appear, please open an issue.

A list of all the queries will be available here soon.
An export to CSV file is planned in order to manipulate data on Excel.

## How to use it

Coming soon.