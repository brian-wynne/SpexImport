CREATE TABLE IF NOT EXISTS headernames_temp
(
	headerid INT,
	name TEXT,
	localeid SMALLINT,
	PRIMARY KEY(headerid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);