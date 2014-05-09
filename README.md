EPiServer License Exporter
==========================

Syncs remote data from EPiServer's service API with a local SQL Server database.

This is a "hard" sync. It's one way, and it actually drops all the tables in the database before writing new data. So this only allows a local copy of your EPiServer license data for querying and reporting. It does not push anything into EPiServer's system, and you shouldn't extend the database tables with your own data, because they get wiped out prior to rewriting.