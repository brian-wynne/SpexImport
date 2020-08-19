CREATE TABLE IF NOT EXISTS locales_temp
(
	localeid SMALLINT,
	isactive TINYINT,
	languagecode VARCHAR(5),
	countrycode VARCHAR(5),
	name TEXT
);