/*
 Navicat Premium Data Transfer

 Source Server         : postgres
 Source Server Type    : PostgreSQL
 Source Server Version : 140000
 Source Host           : localhost:5432
 Source Catalog        : Cubirds-Online
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 140000
 File Encoding         : 65001

 Date: 03/08/2022 12:44:49
*/


-- ----------------------------
-- Sequence structure for asset_bundle_info_id_sequence
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."asset_bundle_info_id_sequence";
CREATE SEQUENCE "public"."asset_bundle_info_id_sequence" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Table structure for asset_bundle_info
-- ----------------------------
DROP TABLE IF EXISTS "public"."asset_bundle_info";
CREATE TABLE "public"."asset_bundle_info" (
  "id" int8 NOT NULL,
  "info_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "asset_bundle_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "package_date" timestamp(0) NOT NULL,
  "asset_bundle_local_file_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "manifest_local_file_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL
)
;
COMMENT ON COLUMN "public"."asset_bundle_info"."info_name" IS '数据包信息的名称，也是显示名，便于维护时区分的名称';
COMMENT ON COLUMN "public"."asset_bundle_info"."asset_bundle_name" IS '数据包名称，区分数据包的唯一标志';
COMMENT ON COLUMN "public"."asset_bundle_info"."package_date" IS '打包时间，区分资源包新旧的标志';
COMMENT ON COLUMN "public"."asset_bundle_info"."asset_bundle_local_file_name" IS '本地保存的资源包文件的名称';
COMMENT ON COLUMN "public"."asset_bundle_info"."manifest_local_file_name" IS '本地保存的 manifest 文件的名称';

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."asset_bundle_info_id_sequence"', 1, true);

-- ----------------------------
-- Primary Key structure for table asset_bundle_info
-- ----------------------------
ALTER TABLE "public"."asset_bundle_info" ADD CONSTRAINT "asset_bundle_info_pkey" PRIMARY KEY ("id");
