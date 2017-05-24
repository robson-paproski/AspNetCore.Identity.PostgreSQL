CREATE TABLE public."AspNetRoles" (
  "Id" VARCHAR(128) NOT NULL,
  "Name" TEXT NOT NULL,
  "ConcurrencyStamp" TEXT,
  CONSTRAINT "AspNetRoles_pkey" PRIMARY KEY("Id")
) 
WITH (oids = false);

CREATE TABLE public."AspNetUsers" (
  "Id" UUID NOT NULL,
  "UserName" TEXT NOT NULL,
  "PasswordHash" TEXT,
  "SecurityStamp" TEXT,
  "Email" TEXT,
  "EmailConfirmed" BOOLEAN DEFAULT false NOT NULL,
  "PhoneNumber" TEXT,
  "PhoneNumberConfirmed" BOOLEAN DEFAULT false NOT NULL,
  "TwoFactorEnabled" BOOLEAN DEFAULT false NOT NULL,
  "LockoutEndDateUtc" TIMESTAMP WITH TIME ZONE,
  "LockoutEnabled" BOOLEAN DEFAULT false NOT NULL,
  "AccessFailedCount" INTEGER NOT NULL,
  "ConcurrencyStamp" TEXT,
  CONSTRAINT "AspNetUsers_pkey" PRIMARY KEY("Id")
) 
WITH (oids = false);


CREATE TABLE public."AspNetUserClaims" (
  "Id" SERIAL,
  "ClaimType" TEXT,
  "ClaimValue" TEXT,
  "UserId" UUID NOT NULL,
  CONSTRAINT "AspNetUserClaims_pkey" PRIMARY KEY("Id"),
  CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_User_Id" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers"("Id")
    ON DELETE CASCADE
    ON UPDATE NO ACTION
    NOT DEFERRABLE
) 
WITH (oids = false);


CREATE TABLE public."AspNetUserLogins" (
  "UserId" UUID NOT NULL,
  "LoginProvider" TEXT NOT NULL,
  "ProviderKey" TEXT NOT NULL,
  "ProviderDisplayName" TEXT,
  CONSTRAINT "AspNetUserLogins_pkey" PRIMARY KEY("UserId", "LoginProvider", "ProviderKey"),
  CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers"("Id")
    ON DELETE CASCADE
    ON UPDATE NO ACTION
    NOT DEFERRABLE
) 
WITH (oids = false);


CREATE TABLE public."AspNetUserRoles" (
  "UserId" UUID NOT NULL,
  "RoleId" VARCHAR(128) NOT NULL,
  CONSTRAINT "AspNetUserRoles_pkey" PRIMARY KEY("UserId", "RoleId"),
  CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId")
    REFERENCES public."AspNetRoles"("Id")
    ON DELETE CASCADE
    ON UPDATE NO ACTION
    NOT DEFERRABLE,
  CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers"("Id")
    ON DELETE CASCADE
    ON UPDATE NO ACTION
    NOT DEFERRABLE
) 
WITH (oids = false);

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles"
  USING btree ("RoleId" COLLATE pg_catalog."default");

CREATE INDEX "IX_AspNetUserRoles_UserId" ON public."AspNetUserRoles"
  USING btree ("UserId");

CREATE INDEX "IX_AspNetUserClaims_UserId"	ON "AspNetUserClaims"	("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId"	ON "AspNetUserLogins"	("UserId");

