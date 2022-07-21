package org.mtc.cubirdsonline.service;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.ProcessHandle.Info;
import java.net.URLEncoder;
import java.util.Date;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collector;
import java.util.stream.Collectors;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.boot.SessionFactoryBuilder;
import org.hibernate.boot.internal.SessionFactoryBuilderImpl;
import org.mtc.cubirdsonline.domain.AssetBundleInfo;
import org.mtc.cubirdsonline.repository.AssetBundleInfoRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;

@Service
public class HotUpdateService {
	
	private static final Logger log = LoggerFactory.getLogger(HotUpdateService.class);

	/**
	 * 本地文件的存储根目录
	 */
	@Value("${local-files.local-files-root-path}")
	private String localFilesRootPath;
	/**
	 * 热更新文件的存储目录
	 */
	@Value("${local-files.hot-update-path}")
	private String hotUpdateFilePath;
	
	@Autowired
	private AssetBundleInfoRepository assetBundleInfoRepository;

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
	public AssetBundleInfo uploadAssetBundle(MultipartFile assetBundleFile, MultipartFile manifestFile,
			String assetBundleName, String displayName, HttpServletRequest request) throws IllegalStateException, IOException {

		log.info("上传资源包 {}，显示名为 {}", assetBundleName, displayName);
		
		// 获取保存路径
		String fileSavePath = getAssetBundleFileSavePath(request);
		
		log.debug("存放文件的位置 = {}", fileSavePath);

		// 如果这个路径不存在则创建出来，因为这个路径也就只会创建一次这里干脆写一个耗费资源多一点但是代码量少比较不影响查代码的写法
		if(!new File(fileSavePath).exists()){
			new File(fileSavePath).mkdirs();
        }
		
        // 把文件名和保存路径拼接起来，获得资源包在本地保存的路径，后缀添加 UUID 防重名
        File uploadAssetBundleFile = new File(fileSavePath + File.separator + assetBundleFile.getOriginalFilename() + "-" + UUID.randomUUID().toString());
        // 把资源包文件保存下来
        assetBundleFile.transferTo(uploadAssetBundleFile);
     
        // manifest 文件同样这样处理，但名字里多加一个 manifest 便于区分
        File uploadManifestFile = new File(fileSavePath + File.separator + assetBundleFile.getOriginalFilename() + "-manifest-" + UUID.randomUUID().toString());
        manifestFile.transferTo(uploadManifestFile);
        
        // 准备一个资源包信息对象用于存储数据
		AssetBundleInfo assetBundleInfo = new AssetBundleInfo();

		// 记录本地文件的名称
		assetBundleInfo.setAssetBundleLocalFileName(uploadAssetBundleFile.getName());
		assetBundleInfo.setManifestLocalFileName(uploadManifestFile.getName());

		// 设置名称
		assetBundleInfo.setInfoName(displayName);
		assetBundleInfo.setAssetBundleName(assetBundleName);

		// 以当前时间作为打包时间
		assetBundleInfo.setPackageDate(new Date());
		
		// 交给 Mapping 保存
		assetBundleInfo = assetBundleInfoRepository.save(assetBundleInfo);

		return assetBundleInfo;
	}

	/**
	 * 获取 AssetBundle 文件保存的路径
	 * 
	 * @param request
	 * @return
	 */
	private String getAssetBundleFileSavePath(HttpServletRequest request) {
		// 获取保存文件的路径，通过本地路径、文件存储根目录、热更新文件存储目录拼接得来，之后把斜杠换为跨平台分隔符
		String fileSavePath = (request.getServletContext().getRealPath("/") + localFilesRootPath + hotUpdateFilePath).replace("/", File.separator);
		return fileSavePath;
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
	public void downloadAssetBundle(String assetBundleName, HttpServletRequest request,
			HttpServletResponse response) throws IOException {
		
		log.info("下载资源包 {}", assetBundleName);
		
		// 下载文件
		downloadAssetBundleFile(assetBundleName, false, request, response);
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
	public void downloadManifest(String assetBundleName, HttpServletRequest request, HttpServletResponse response)
			throws IOException {
		
		log.info("下载资源包 {} 对应的 manifest 文件", assetBundleName);

		// 下载文件
		downloadAssetBundleFile(assetBundleName, true, request, response);
	}
	
	/**
	 * 下载文件
	 * 
	 * @param assetBundleName
	 * @param isManifest 如果这里传 true 则下载 manifest 文件，否则下载资源包文件
	 * @param request
	 * @throws IOException 
	 */
	private void downloadAssetBundleFile(String assetBundleName, Boolean isManifest, HttpServletRequest request, HttpServletResponse response) throws IOException {

		// 根据名字获取最新的资源包信息
		AssetBundleInfo assetBundleInfo = assetBundleInfoRepository.findFirstOneByAssetBundleNameOrderByPackageDateDesc(assetBundleName);
		
		// 获取存放文件的位置
		String fileSavePath = getAssetBundleFileSavePath(request);
		
		// 根据是否下载 manifest 文件确认使用哪个本地文件名
		String localFileNameString = isManifest ? assetBundleInfo.getManifestLocalFileName() : assetBundleInfo.getAssetBundleLocalFileName();

		// 把文件名和存储位置拼接起来，就是要下载的文件的完整路径
        File downloadFile = new File(fileSavePath + File.separator + localFileNameString);
        
        log.debug("下载文件路径 = {}", downloadFile.getAbsolutePath());
        
        // 文件不存在则不进行后续操作
        if(!downloadFile.exists()) {
        	log.warn("要下载的文件并没有在服务器上存储");
        	return;
        }
        
        // 设置相应的连接类型为强制下载
        response.setContentType("application/force-download");
        
        // 把要下载的文件转为输入流
        InputStream inputStreamFromFile = new FileInputStream(downloadFile);
        
        // 根据是否下载 manifest 文件确认下载后的文件名称
        String downoadFileName = isManifest ? assetBundleInfo.getAssetBundleName() + ".manifest" : assetBundleInfo.getAssetBundleName();
        
        // 设置相应头，设置文件名为 UTF8 编码的资源包名
        response.setHeader("Content-Disposition", "attachment;filename=" + URLEncoder.encode(downoadFileName, "UTF-8"));
        
        // 设置相应长度为输入流的可用长度，就是说这个文件能传多少就设为多少
        response.setContentLength(inputStreamFromFile.available());

        // 获取相应的输出流
        OutputStream outStreamToResponse = response.getOutputStream();

        // 经典版的分块依次输出直到输出完毕
        byte[] bytes = new byte[1024];
        int lenth = 0;
        while((lenth = inputStreamFromFile.read(bytes))!=-1){
            outStreamToResponse.write(bytes, 0, lenth);
        }
        
        // 不要忘了刷新，刷新了才是有效的
        outStreamToResponse.flush();
        
        // 关闭两个流
        outStreamToResponse.close();
        inputStreamFromFile.close();
	}
	
	/**
	 * 获取资源包和更新时间的接口，返回所有资源包和资源包的最新更新时间
	 * 
	 * @return
	 */
	public Map<String, Date> getHotUpdateInfo() {
		
		log.info("获取所有热更新资源包信息");
		
		// 获取每种资源包的最新版信息
		List<AssetBundleInfo> assetBundleInfos = assetBundleInfoRepository.findNewestGroupByAssetBundleName();

		// 转为 <资源包名称, 打包时间> 映射表
		Map<String, Date> assetBundlePackageDates = assetBundleInfos.stream().collect(Collectors.toMap(info -> info.getAssetBundleName(), info -> info.getPackageDate()));
		
		// 返回
		return assetBundlePackageDates;
	}
}
