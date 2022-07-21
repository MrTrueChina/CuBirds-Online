package org.mtc.cubirdsonline.domain;

import java.util.Date;

import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

import lombok.Data;

@Entity
@Data
public class AssetBundleInfo {
	/**
	 * 数据包的 ID
	 */
	@Id
	// 这个数据自动生成，生成方式是 SEQUENCE（序列），生成器的名字是“asset_bundle_info_id_sequence”
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "asset_bundle_info_id_sequence")
	// 这个数据使用序列生成，生成序列名字是“asset_bundle_info_id_sequence”，步长是 1
	@SequenceGenerator(name = "asset_bundle_info_id_sequence", allocationSize = 1)
	private Long id;
	/**
	 * 数据包信息的名称，这个是便于区分的名称
	 */
	private String infoName;
	/**
	 * 数据包的名称，这是指打包后的数据包的名称，也是区分数据包文件究竟对应哪个数据包的唯一识别值
	 */
	private String assetBundleName;
	/**
	 * 数据包打包的时间，以打包时间确认最新的数据包
	 */
	private Date packageDate;
	/**
	 * 本地保存的资源包文件名称
	 */
	private String assetBundleLocalFileName;
	/**
	 * 本地保存的 manifest 文件名称
	 */
	private String manifestLocalFileName;
}
