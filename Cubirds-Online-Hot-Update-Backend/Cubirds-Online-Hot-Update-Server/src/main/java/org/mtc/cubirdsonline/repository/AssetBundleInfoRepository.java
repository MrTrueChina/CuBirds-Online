package org.mtc.cubirdsonline.repository;

import java.util.Date;
import java.util.List;
import java.util.Map;

import org.mtc.cubirdsonline.domain.AssetBundleInfo;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface AssetBundleInfoRepository extends JpaRepository<AssetBundleInfo, Long>{
	
	/**
	 * 获取指定资源包的最新版信息，规则是：获取 第一个 指定资源包名称的信息 按照打包时间倒序排序
	 * 
	 * @param assetBundleName
	 * @return
	 */
	AssetBundleInfo findFirstOneByAssetBundleNameOrderByPackageDateDesc(String assetBundleName);
	
	/**
	 * 获取所有资源包的最新版信息
	 * 
	 * @return
	 */
	@Query(value = "SELECT\r\n"
			+ "	full_info.* \r\n"
			+ "FROM\r\n"
			+ "	asset_bundle_info AS full_info\r\n"
			+ "	JOIN ( SELECT asset_bundle_name, MAX ( package_date ) AS package_date FROM asset_bundle_info GROUP BY asset_bundle_name ) AS newest_info ON full_info.asset_bundle_name = newest_info.asset_bundle_name \r\n"
			+ "	AND full_info.package_date = newest_info.package_date",
			nativeQuery = true)
	List<AssetBundleInfo> findNewestGroupByAssetBundleName();
}
