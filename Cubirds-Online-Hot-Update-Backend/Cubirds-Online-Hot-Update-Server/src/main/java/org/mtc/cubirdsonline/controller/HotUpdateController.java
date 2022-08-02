package org.mtc.cubirdsonline.controller;

import java.io.IOException;
import java.util.Date;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.mtc.cubirdsonline.service.HotUpdateService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;

@Controller
@RequestMapping("/hotUpdate/")
public class HotUpdateController {
	
	private static final Logger log = LoggerFactory.getLogger(HotUpdateController.class);

	@Autowired
	private HotUpdateService hotUpdateService;

	/**
	 * 上传一个资源包
	 * 
	 * @param assetBundleFile 资源包文件
	 * @param manifestFile    资源包的 manifest 文件
	 * @param assetBundleName 资源包名称，这是区分资源包的唯一标志
	 * @param displayName     资源包的显示名，便于维护的时候进行区分
	 * @param request
	 * @return
	 * @throws IOException 
	 * @throws IllegalStateException 
	 */
	@RequestMapping(value = "upload/upload", method = RequestMethod.POST)
	public ResponseEntity<String> uploadAssetBundle(@RequestParam("assetBundleFile") MultipartFile assetBundleFile,
			@RequestParam("manifestFile") MultipartFile manifestFile,
			@RequestParam("assetBundleName") String assetBundleName, @RequestParam("displayName") String displayName,
			HttpServletRequest request) throws IllegalStateException, IOException {

		log.info("上传资源包 {}，显示名为 {}", assetBundleName, displayName);
		
		// 交给 Service 处理
		hotUpdateService.uploadAssetBundle(assetBundleFile, manifestFile, assetBundleName, displayName, request);

		return ResponseEntity.ok("Uploaded");
	}

	/**
	 * 下载资源包文件，总是下载最新版
	 * 
	 * @param assetBundleName 资源包名
	 * @param request
	 * @param response
	 * @return
	 * @throws IOException 
	 */
	@RequestMapping(value = "downloadAssetBundle", method = RequestMethod.GET)
	public ResponseEntity<String> downloadAssetBundle(@RequestParam("assetBundleName") String assetBundleName,
			HttpServletRequest request, HttpServletResponse response) throws IOException {
		
		log.info("下载资源包 {}", assetBundleName);
		
		// 交给 Service 进行
		hotUpdateService.downloadAssetBundle(assetBundleName, request, response);
		
		return ResponseEntity.ok("Downloaded");
	}

	/**
	 * 下载资源包对应的 manifest 文件，总是下载最新版
	 * 
	 * @param assetBundleName 资源包名
	 * @param request
	 * @param response
	 * @return
	 * @throws IOException 
	 */
	@RequestMapping(value = "downloadManifest", method = RequestMethod.GET)
	public ResponseEntity<String> downloadManifest(@RequestParam("assetBundleName") String assetBundleName,
			HttpServletRequest request, HttpServletResponse response) throws IOException {
		
		log.info("下载资源包 {} 对应的 manifest 文件", assetBundleName);
		
		// 交给 Service 进行
		hotUpdateService.downloadManifest(assetBundleName, request, response);
		
		return ResponseEntity.ok("Downloaded");
	}

	/**
	 * 获取资源包和更新时间的接口，返回所有资源包和资源包的最新更新时间
	 * 
	 * @return
	 */
	@RequestMapping(value = "check", method = RequestMethod.GET)
	public ResponseEntity<Map<String, Date>> getHotUpdateInfo() {
		
		log.info("获取所有热更新资源包信息");
		
		// 交由 Service 获取
		Map<String, Date> hotUpdateInfo = hotUpdateService.getHotUpdateInfo();
		
		// 返回
		return ResponseEntity.ok(hotUpdateInfo);
	}
}
