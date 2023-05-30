<template>
    <pageMain>
        <search-bar :fold="isFold">
            <template #default="{ fold }">
                <el-form :model="search" size="default" label-width="120px">
                    <el-row>
                        <el-col :span="12">
                            <el-form-item label="姓名 / 手机号">
                                <el-input v-model="search.name" placeholder="请输入姓名或者手机号，支持模糊查询" clearable />
                            </el-form-item>
                        </el-col>
                    </el-row>
                    <el-row v-show="!fold">
                        <el-col :span="6">
                            <el-form-item label="部门">
                                <el-select v-model="search.department_id" clearable placeholder="请选择部门">
                                    <el-option label="技术部" :value="1" />
                                    <el-option label="设计部" :value="2" />
                                    <el-option label="行政部" :value="3" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :span="6">
                            <el-form-item label="职位">
                                <el-select v-model="search.department_job_id" clearable placeholder="请选择职位">
                                    <el-option label="程序员" :value="1" />
                                    <el-option label="设计师" :value="2" />
                                    <el-option label="人事" :value="3" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :span="6">
                            <el-form-item label="角色">
                                <el-select v-model="search.role_id" clearable placeholder="请选择角色">
                                    <el-option label="主管" :value="1" />
                                    <el-option label="普通职员" :value="2" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row>
                    <el-row v-show="!fold">
                        <el-col :span="24">
                            <el-form-item label="角色">
                                <el-checkbox :value="true">
                                    备选项
                                </el-checkbox>
                                <el-checkbox :value="false">
                                    备选项
                                </el-checkbox>
                            </el-form-item>
                        </el-col>
                    </el-row>
                    <el-form-item>
                        <el-button type="primary">
                            <template #icon>
                                <el-icon>
                                    <svg-icon name="ep:search" />
                                </el-icon>
                            </template>
                            筛选
                        </el-button>
                        <el-button>导出</el-button>
                        <el-button type="primary" link>
                            查看已导出记录
                        </el-button>
                    </el-form-item>
                </el-form>
            </template>
        </search-bar>
        <el-table :data="tableData" border style="width: 100%;margin-top:50px;">
            <el-table-column prop="date" label="Date" width="180" />
            <el-table-column prop="name" label="Name" width="180" />
            <el-table-column prop="address" label="Address" />
        </el-table>
    </pageMain>
</template>
  
<script lang="ts" setup>
let search = ref({
    name: "",
    department_id: "",
    department_job_id: "",
    role_id: ""
}) 
const isFold = ref(false)

const tableData = [
    {
        date: '2016-05-03',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-02',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-04',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-01',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
]
</script>