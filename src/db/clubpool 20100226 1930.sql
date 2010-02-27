USE clubpool;
INSERT INTO `hibernate_unique_key` (`next_hi`) VALUES  (6);
INSERT INTO `roles` (`Id`,`Name`) VALUES 
 (4004,'Administrators'),
 (4005,'Users');

INSERT INTO `users` (`Id`,`Username`,`Password`,`PasswordSalt`,`Email`,`IsApproved`) VALUES 
 (5005,'admin','admin','OAP13hkFvfqVaVGTxr6JgQ==','admin@email.com',1),
 (5006,'user','user','pco9kQLrRswjaxFTZqlMKw==','user@email.com',1);

INSERT INTO `users_roles` (`UserId`,`RoleId`) VALUES 
 (5006,4005),
 (5005,4005),
 (5005,4004);