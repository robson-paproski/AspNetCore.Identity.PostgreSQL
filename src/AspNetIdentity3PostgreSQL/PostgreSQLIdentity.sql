 CREATE TABLE "AspNetRoles" ( 
  "Id" varchar(128) NOT NULL,
  "Name" TEXT NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
  "Id" UUID NOT NULL,
  "UserName" TEXT NOT NULL,
  "PasswordHash" TEXT,
  "SecurityStamp" TEXT,
  "Email" TEXT DEFAULT NULL,
  "EmailConfirmed" boolean NOT NULL DEFAULT false,
  "PhoneNumber" TEXT,
  "PhoneNumberConfirmed" boolean NOT NULL DEFAULT false,
  "TwoFactorEnabled"     boolean NOT NULL DEFAULT false,
  "LockoutEndDateUtc"    timestamptz       NULL,
  "LockoutEnabled"       boolean NOT NULL DEFAULT false,
  "AccessFailedCount"    INT            NOT NULL,  
  PRIMARY KEY ("Id")
);


CREATE TABLE "AspNetUserClaims" ( 
  "Id" SERIAL,
  "ClaimType" TEXT NULL,
  "ClaimValue" TEXT NULL,
  "UserId" UUID NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUserLogins" ( 
  "UserId" UUID NOT NULL,
  "LoginProvider" TEXT NOT NULL,
  "ProviderKey" TEXT NOT NULL,
  PRIMARY KEY ("UserId", "LoginProvider", "ProviderKey")
);

CREATE TABLE "AspNetUserRoles" ( 
  "UserId" UUID NOT NULL,
  "RoleId" varchar(128) NOT NULL,
  PRIMARY KEY ("UserId", "RoleId")
);

CREATE INDEX "IX_AspNetUserClaims_UserId"	ON "AspNetUserClaims"	("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId"	ON "AspNetUserLogins"	("UserId");
CREATE INDEX "IX_AspNetUserRoles_RoleId"	ON "AspNetUserRoles"	("RoleId");
CREATE INDEX "IX_AspNetUserRoles_UserId"	ON "AspNetUserRoles"	("UserId");

ALTER TABLE "AspNetUserClaims"
  ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_User_Id" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserLogins"
  ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles"
  ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles"
  ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;