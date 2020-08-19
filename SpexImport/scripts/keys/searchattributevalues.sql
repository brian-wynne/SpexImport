ALTER TABLE searchattributevalues_temp
	ADD PRIMARY KEY(valueid),
	ADD FOREIGN KEY(unitid) REFERENCES units(unitid);