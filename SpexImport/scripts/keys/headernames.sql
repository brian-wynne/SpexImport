ALTER TABLE headernames_temp
	ADD PRIMARY KEY(headerid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);