CREATE TABLE IF NOT EXISTS categorynames_temp
(
	categoryid INT,
	name TEXT,
	localeid SMALLINT,
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);
