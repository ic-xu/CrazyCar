package com.tastsong.crazycar.service;

import java.util.List;

import com.tastsong.crazycar.model.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.tastsong.crazycar.mapper.MatchMapper;

@Service
public class MatchService {
    @Autowired
    private UserService userService;

    @Autowired
    private EquipService equipService;

    @Autowired
    private MatchMapper matchMapper;


    public boolean canWade(int eid){
        return equipService.getEquipByEid(eid).isCan_wade();
    }


    public boolean isBreakRecord(MatchRecordModel recordModel) {
        if (recordModel.complete_time == -1) {
			return false;
		} 
        int minTime = matchMapper.getMiniCompleteTime(recordModel.uid, recordModel.cid);
        if(minTime == 0){
            minTime = -1;
        }
		if (minTime == -1 && recordModel.complete_time != -1){
			return true;
		}		
		
		return recordModel.complete_time < minTime;
    }

    public void insertRecord(MatchRecordModel recordModel) {
        matchMapper.insertRecord(recordModel);
    }

    // private void initRank(int uid, int cid){
    //     matchMapper.delMatchRank(uid, cid);
    //     matchMapper.initMatchRank(uid, cid);
    // }

    public List<MatchRankModel> getRankList(int uid, int cid){
        // initRank(uid, cid);
        // List<MatchRankModel> rankModels =  matchMapper.getMatchRankList(uid, cid);
        List<MatchRankModel> rankModels =  matchMapper.getMatchRankListByCid(cid);
        for (MatchRankModel rankModel : rankModels) {
            int userId = rankModel.uid;
            UserModel userModel = userService.getUserByUid(userId);
            rankModel.aid = userModel.getAid();
            rankModel.user_name = userModel.getUser_name();
        }
        return rankModels;
    }
}
