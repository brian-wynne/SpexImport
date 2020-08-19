ALTER TABLE productfeaturebullets_temp
	ADD PRIMARY KEY(uniqueid),
	ADD FOREIGN KEY(productid) REFERENCES product(productid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);