ALTER TABLE productkeywords_temp
	ADD FOREIGN KEY(productid) REFERENCES product(productid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);