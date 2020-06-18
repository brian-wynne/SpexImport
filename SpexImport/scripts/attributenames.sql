CREATE TABLE IF NOT EXISTS attributenames_temp
(
	attributeid BIGINT,
	name TEXT,
	localeid SMALLINT,
	PRIMARY KEY(attributeid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);