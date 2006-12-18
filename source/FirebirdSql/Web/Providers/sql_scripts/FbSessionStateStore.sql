SET SQL DIALECT 3;

SET NAMES NONE;

CREATE TABLE SESSIONS (
    SESSION_ID        VARCHAR(80) NOT NULL,
    APPLICATION_NAME  VARCHAR(255) CHARACTER SET UNICODE_FSS NOT NULL,
    CREATED           TIMESTAMP,
    EXPIRES           TIMESTAMP,
    LOCK_DATE         TIMESTAMP,
    LOCK_ID           INTEGER,
    TIMEOUT           INTEGER,
    LOCKED            SMALLINT,
    SESSION_ITEMS     BLOB SUB_TYPE 1 SEGMENT SIZE 4096,
    FLAGS             INTEGER
);

CREATE UNIQUE INDEX SESSIONS_IDX1 ON SESSIONS (SESSION_ID, APPLICATION_NAME); 