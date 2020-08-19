ALTER TABLE attributenames_temp
	ADD PRIMARY KEY(attributeid), 
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);